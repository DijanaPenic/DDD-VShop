using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using MediatR;

namespace VShop.SharedKernel.Infrastructure.Messaging.Events.Publishing
{
    public class EventBus : IEventBus
    {
        public EventBus(ServiceFactory serviceFactory)
        {
            _publishStrategies[EventPublishStrategy.Async] = new ApplicationMediator(serviceFactory, AsyncContinueOnException);
            _publishStrategies[EventPublishStrategy.ParallelNoWait] = new ApplicationMediator(serviceFactory, ParallelNoWait);
            _publishStrategies[EventPublishStrategy.ParallelWhenAll] = new ApplicationMediator(serviceFactory, ParallelWhenAll);
            _publishStrategies[EventPublishStrategy.ParallelWhenAny] = new ApplicationMediator(serviceFactory, ParallelWhenAny);
            _publishStrategies[EventPublishStrategy.SyncContinueOnException] = new ApplicationMediator(serviceFactory, SyncContinueOnException);
            _publishStrategies[EventPublishStrategy.SyncStopOnException] = new ApplicationMediator(serviceFactory, SyncStopOnException);
        }

        private readonly IDictionary<EventPublishStrategy, IMediator> _publishStrategies = new Dictionary<EventPublishStrategy, IMediator>();
        private static EventPublishStrategy DefaultStrategy => EventPublishStrategy.SyncContinueOnException;

        public Task Publish<TNotification>(TNotification notification)
            => Publish(notification, DefaultStrategy, default);

        public Task Publish<TNotification>(TNotification notification, EventPublishStrategy strategy)
            => Publish(notification, strategy, default);

        public Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken)
            => Publish(notification, DefaultStrategy, cancellationToken);

        public Task Publish<TNotification>(TNotification notification, EventPublishStrategy strategy, CancellationToken cancellationToken)
        {
            if (!_publishStrategies.TryGetValue(strategy, out IMediator mediator))
            {
                throw new ArgumentException($"Unknown strategy: {strategy}");
            }

            return mediator.Publish(notification, cancellationToken);
        }

        private static Task ParallelWhenAll
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

        private static Task ParallelWhenAny
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

        private static Task ParallelNoWait
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

        private static async Task AsyncContinueOnException
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

        private static async Task SyncStopOnException
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

        private static async Task SyncContinueOnException
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