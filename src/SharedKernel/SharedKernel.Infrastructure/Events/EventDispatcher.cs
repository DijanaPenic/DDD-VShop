using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Reflection;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

using VShop.SharedKernel.Infrastructure.Events.Contracts;
using VShop.SharedKernel.Infrastructure.Contexts.Contracts;

namespace VShop.SharedKernel.Infrastructure.Events
{
    internal class EventDispatcher : IEventDispatcher
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IContextAdapter _contextAdapter;
        private readonly IDictionary<EventDispatchStrategy, EventMediator> _publishStrategies = 
            new Dictionary<EventDispatchStrategy, EventMediator>();

        public EventDispatcher
        (
            IServiceProvider serviceProvider,
            IContextAdapter contextAdapter
        )
        {
            _serviceProvider = serviceProvider;
            _contextAdapter = contextAdapter;
            
            _publishStrategies[EventDispatchStrategy.Async] = 
                new EventMediator(AsyncContinueOnExceptionAsync);
            
            _publishStrategies[EventDispatchStrategy.ParallelNoWait] =
                new EventMediator(ParallelNoWaitAsync);
            
            _publishStrategies[EventDispatchStrategy.ParallelWhenAll] = 
                new EventMediator(ParallelWhenAllAsync);
            
            _publishStrategies[EventDispatchStrategy.ParallelWhenAny] = 
                new EventMediator(ParallelWhenAnyAsync);
            
            _publishStrategies[EventDispatchStrategy.SyncContinueOnException] = 
                new EventMediator(SyncContinueOnExceptionAsync);
            
            _publishStrategies[EventDispatchStrategy.SyncStopOnException] = 
                new EventMediator(SyncStopOnExceptionAsync);
        }

        public Task PublishAsync
        (
            IBaseEvent @event,
            CancellationToken cancellationToken
        ) => PublishAsync(@event, EventDispatchStrategy.SyncStopOnException, cancellationToken);

        public async Task PublishAsync
        (
            IBaseEvent @event,
            EventDispatchStrategy strategy,
            CancellationToken cancellationToken
        ) 
        {
            if (!_publishStrategies.TryGetValue(strategy, out EventMediator mediator))
                throw new ArgumentException($"Unknown strategy: {strategy}");
            
            _contextAdapter.ChangeContext(@event);
            
            using IServiceScope scope = _serviceProvider.CreateScope();
            
            Type handlerType = typeof(IEventHandler<>).MakeGenericType(@event.GetType());
            IEnumerable<object> handlers =  scope.ServiceProvider.GetServices(handlerType);
            MethodInfo method = handlerType.GetMethod(nameof(IEventHandler<IBaseEvent>.HandleAsync));

            if (method is null) throw new InvalidOperationException("Command handler is invalid.");

            IEnumerable<Func<Task>> tasks = handlers
                .Select(handler => GetHandlerTask(@event, method, handler, cancellationToken));
            
            await mediator.Publish(tasks);
        }

        private static Func<Task> GetHandlerTask
        (
            IBaseEvent @event,
            MethodBase method,
            object handler,
            CancellationToken cancellationToken
        ) => () => (Task) method.Invoke(handler, new object[] {@event, cancellationToken});

        private static Task ParallelWhenAllAsync(IEnumerable<Func<Task>> handlers)
            => Task.WhenAll(handlers.Select(handler => handler()));

        private static Task ParallelWhenAnyAsync(IEnumerable<Func<Task>> handlers) 
            => Task.WhenAny(handlers.Select(handler => handler()));

        private static Task ParallelNoWaitAsync(IEnumerable<Func<Task>> handlers)
        {
            foreach (Func<Task> handler in handlers) handler();
            return Task.CompletedTask;
        }

        private static async Task AsyncContinueOnExceptionAsync(IEnumerable<Func<Task>> handlers)
        {
            List<Task> tasks = new();
            List<Exception> exceptions = new();

            foreach (Func<Task> handler in handlers)
            {
                try
                {
                    tasks.Add(handler());
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

        private static async Task SyncStopOnExceptionAsync(IEnumerable<Func<Task>> handlers)
        {
            foreach (Func<Task> handler in handlers)
                await handler().ConfigureAwait(false);
        }

        private static async Task SyncContinueOnExceptionAsync(IEnumerable<Func<Task>> handlers)
        {
            List<Exception> exceptions = new();

            foreach (Func<Task> handler in handlers)
            {
                try
                {
                    await handler().ConfigureAwait(false);
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