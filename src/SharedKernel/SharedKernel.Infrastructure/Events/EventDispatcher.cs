using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using MediatR;

using VShop.SharedKernel.Infrastructure.Dispatchers;
using VShop.SharedKernel.Infrastructure.Events.Contracts;

namespace VShop.SharedKernel.Infrastructure.Events
{
    public class EventDispatcher : IEventDispatcher
    {
        public EventDispatcher(ServiceFactory serviceFactory)
        {
            _publishStrategies[NotificationDispatchStrategy.Async] = 
                new NotificationMediator(serviceFactory, AsyncContinueOnExceptionAsync);
            
            _publishStrategies[NotificationDispatchStrategy.ParallelNoWait] =
                new NotificationMediator(serviceFactory, ParallelNoWaitAsync);
            
            _publishStrategies[NotificationDispatchStrategy.ParallelWhenAll] = 
                new NotificationMediator(serviceFactory, ParallelWhenAllAsync);
            
            _publishStrategies[NotificationDispatchStrategy.ParallelWhenAny] = 
                new NotificationMediator(serviceFactory, ParallelWhenAnyAsync);
            
            _publishStrategies[NotificationDispatchStrategy.SyncContinueOnException] = 
                new NotificationMediator(serviceFactory, SyncContinueOnExceptionAsync);
            
            _publishStrategies[NotificationDispatchStrategy.SyncStopOnException] = 
                new NotificationMediator(serviceFactory, SyncStopOnExceptionAsync);
        }

        private readonly IDictionary<NotificationDispatchStrategy, IMediator> _publishStrategies = 
            new Dictionary<NotificationDispatchStrategy, IMediator>();
        
        private static NotificationDispatchStrategy DefaultStrategy 
            => NotificationDispatchStrategy.SyncContinueOnException;

        public Task PublishAsync<TNotification>(TNotification notification) 
            where TNotification : INotification
            => PublishAsync(notification, DefaultStrategy, default);

        public Task PublishAsync<TNotification>(TNotification notification, NotificationDispatchStrategy strategy)
            where TNotification : INotification
            => PublishAsync(notification, strategy, default);

        public Task PublishAsync<TNotification>(TNotification notification, CancellationToken cancellationToken)
            where TNotification : INotification
            => PublishAsync(notification, DefaultStrategy, cancellationToken);

        public Task PublishAsync<TNotification>
        (
            TNotification notification,
            NotificationDispatchStrategy strategy,
            CancellationToken cancellationToken
        )
            where TNotification : INotification
        {
            if (!_publishStrategies.TryGetValue(strategy, out IMediator mediator))
            {
                throw new ArgumentException($"Unknown strategy: {strategy}");
            }

            return mediator.Publish(notification, cancellationToken);
        }

        private static Task ParallelWhenAllAsync
        (
            IEnumerable<Func<INotification,
            CancellationToken, Task>> handlers,
            INotification notification,
            CancellationToken cancellationToken
        )
        {
            List<Task> tasks = handlers.Select(handler 
                => Task.Run(() => handler(notification, cancellationToken), cancellationToken)).ToList();

            return Task.WhenAll(tasks);
        }

        private static Task ParallelWhenAnyAsync
        (
            IEnumerable<Func<INotification,
            CancellationToken, Task>> handlers,
            INotification notification,
            CancellationToken cancellationToken
        )
        {
            List<Task> tasks = handlers.Select(handler =>
                Task.Run(() => handler(notification, cancellationToken), cancellationToken)).ToList();

            return Task.WhenAny(tasks);
        }

        private static Task ParallelNoWaitAsync
        (
            IEnumerable<Func<INotification,
            CancellationToken, Task>> handlers,
            INotification notification,
            CancellationToken cancellationToken
        )
        {
            foreach (Func<INotification, CancellationToken, Task> handler in handlers)
            {
                Task.Run(() => handler(notification, cancellationToken), cancellationToken);
            }

            return Task.CompletedTask;
        }

        private static async Task AsyncContinueOnExceptionAsync
        (
            IEnumerable<Func<INotification,
            CancellationToken, Task>> handlers,
            INotification notification,
            CancellationToken cancellationToken
        )
        {
            List<Task> tasks = new();
            List<Exception> exceptions = new();

            foreach (Func<INotification, CancellationToken, Task> handler in handlers)
            {
                try
                {
                    tasks.Add(handler(notification, cancellationToken));
                }
                catch (Exception ex) when (ex is not (OutOfMemoryException or StackOverflowException))
                {
                    exceptions.Add(ex);
                }
            }

            try
            {
                await Task.WhenAll(tasks).ConfigureAwait(false);
            }
            catch (AggregateException ex)
            {
                exceptions.AddRange(ex.Flatten().InnerExceptions);
            }
            catch (Exception ex) when (ex is not (OutOfMemoryException or StackOverflowException))
            {
                exceptions.Add(ex);
            }

            if (exceptions.Any())
            {
                throw new AggregateException(exceptions);
            }
        }

        private static async Task SyncStopOnExceptionAsync
        (
            IEnumerable<Func<INotification,
            CancellationToken, Task>> handlers,
            INotification notification,
            CancellationToken cancellationToken
        )
        {
            foreach (Func<INotification, CancellationToken, Task> handler in handlers)
            {
                await handler(notification, cancellationToken).ConfigureAwait(false);
            }
        }

        private static async Task SyncContinueOnExceptionAsync
        (
            IEnumerable<Func<INotification,
            CancellationToken, Task>> handlers,
            INotification notification,
            CancellationToken cancellationToken
        )
        {
            List<Exception> exceptions = new();

            foreach (Func<INotification, CancellationToken, Task> handler in handlers)
            {
                try
                {
                    await handler(notification, cancellationToken).ConfigureAwait(false);
                }
                catch (AggregateException ex)
                {
                    exceptions.AddRange(ex.Flatten().InnerExceptions);
                }
                catch (Exception ex) when (ex is not (OutOfMemoryException or StackOverflowException))
                {
                    exceptions.Add(ex);
                }
            }

            if (exceptions.Any())
            {
                throw new AggregateException(exceptions);
            }
        }
    }
}