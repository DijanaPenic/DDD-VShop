using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using MediatR;

using VShop.SharedKernel.Infrastructure.Contexts.Contracts;
using VShop.SharedKernel.Infrastructure.Events.Contracts;

namespace VShop.SharedKernel.Infrastructure.Events
{
    internal class EventDispatcher : IEventDispatcher
    {
        private readonly IContextAdapter _contextAdapter;
        private readonly IDictionary<EventDispatchStrategy, IMediator> _publishStrategies = 
            new Dictionary<EventDispatchStrategy, IMediator>();

        public EventDispatcher
        (
            ServiceFactory serviceFactory,
            IContextAdapter contextAdapter
        )
        {
            _contextAdapter = contextAdapter;
            
            _publishStrategies[EventDispatchStrategy.Async] = 
                new EventMediator(serviceFactory, AsyncContinueOnExceptionAsync);
            
            _publishStrategies[EventDispatchStrategy.ParallelNoWait] =
                new EventMediator(serviceFactory, ParallelNoWaitAsync);
            
            _publishStrategies[EventDispatchStrategy.ParallelWhenAll] = 
                new EventMediator(serviceFactory, ParallelWhenAllAsync);
            
            _publishStrategies[EventDispatchStrategy.ParallelWhenAny] = 
                new EventMediator(serviceFactory, ParallelWhenAnyAsync);
            
            _publishStrategies[EventDispatchStrategy.SyncContinueOnException] = 
                new EventMediator(serviceFactory, SyncContinueOnExceptionAsync);
            
            _publishStrategies[EventDispatchStrategy.SyncStopOnException] = 
                new EventMediator(serviceFactory, SyncStopOnExceptionAsync);
        }

        public Task PublishAsync<TEvent>
        (
            TEvent @event,
            CancellationToken cancellationToken = default
        ) where TEvent : IBaseEvent
            => PublishAsync(@event, EventDispatchStrategy.SyncStopOnException, cancellationToken);

        public Task PublishAsync<TEvent>
        (
            TEvent @event,
            EventDispatchStrategy strategy,
            CancellationToken cancellationToken = default
        ) where TEvent : IBaseEvent
        {
            if (!_publishStrategies.TryGetValue(strategy, out IMediator mediator))
                throw new ArgumentException($"Unknown strategy: {strategy}");
            
            _contextAdapter.ChangeContext(@event);
            return mediator.Publish(@event, cancellationToken);
        }
        
        private static Task ParallelWhenAllAsync<TEvent>
        (
            IEnumerable<Func<TEvent, CancellationToken, Task>> handlers,
            TEvent @event,
            CancellationToken cancellationToken
        ) where TEvent : INotification
        {
            List<Task> tasks = handlers.Select(handler 
                => Task.Run(() => handler(@event, cancellationToken), cancellationToken)).ToList();

            return Task.WhenAll(tasks);
        }

        private static Task ParallelWhenAnyAsync<TEvent>
        (
            IEnumerable<Func<TEvent, CancellationToken, Task>> handlers,
            TEvent @event,
            CancellationToken cancellationToken
        )
        {
            List<Task> tasks = handlers.Select(handler =>
                Task.Run(() => handler(@event, cancellationToken), cancellationToken)).ToList();

            return Task.WhenAny(tasks);
        }

        private static Task ParallelNoWaitAsync<TEvent>
        (
            IEnumerable<Func<TEvent, CancellationToken, Task>> handlers,
            TEvent @event,
            CancellationToken cancellationToken
        )
        {
            foreach (Func<TEvent, CancellationToken, Task> handler in handlers)
                Task.Run(() => handler(@event, cancellationToken), cancellationToken);

            return Task.CompletedTask;
        }

        private static async Task AsyncContinueOnExceptionAsync<TEvent>
        (
            IEnumerable<Func<TEvent, CancellationToken, Task>> handlers,
            TEvent @event,
            CancellationToken cancellationToken
        )
        {
            List<Task> tasks = new();
            List<Exception> exceptions = new();

            foreach (Func<TEvent, CancellationToken, Task> handler in handlers)
            {
                try
                {
                    tasks.Add(handler(@event, cancellationToken));
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
                throw new AggregateException(exceptions);
        }

        private static async Task SyncStopOnExceptionAsync<TEvent>
        (
            IEnumerable<Func<TEvent, CancellationToken, Task>> handlers,
            TEvent @event,
            CancellationToken cancellationToken
        )
        {
            foreach (Func<TEvent, CancellationToken, Task> handler in handlers)
                await handler(@event, cancellationToken).ConfigureAwait(false);
        }

        private static async Task SyncContinueOnExceptionAsync<TEvent>
        (
            IEnumerable<Func<TEvent, CancellationToken, Task>> handlers,
            TEvent @event,
            CancellationToken cancellationToken
        )
        {
            List<Exception> exceptions = new();

            foreach (Func<TEvent, CancellationToken, Task> handler in handlers)
            {
                try
                {
                    await handler(@event, cancellationToken).ConfigureAwait(false);
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
                throw new AggregateException(exceptions);
        }
    }
}