using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

using VShop.SharedKernel.EventStoreDb;
using VShop.SharedKernel.Domain.ValueObjects;
using VShop.SharedKernel.Infrastructure.Services.Contracts;
using VShop.SharedKernel.EventSourcing.Aggregates;
using VShop.SharedKernel.EventSourcing.Stores.Contracts;
using VShop.SharedKernel.Infrastructure.Contexts.Contracts;
using VShop.SharedKernel.Infrastructure.Dispatchers;
using VShop.SharedKernel.Infrastructure.Events.Contracts;
using VShop.SharedKernel.Infrastructure.Messaging;
using VShop.SharedKernel.Infrastructure.Messaging.Contracts;
using VShop.SharedKernel.Infrastructure.Types;

namespace VShop.SharedKernel.EventSourcing.Stores
{
    public class AggregateStore<TAggregate> : IAggregateStore<TAggregate> where TAggregate : AggregateRoot, new()
    {
        private readonly IClockService _clockService;
        private readonly CustomEventStoreClient _eventStoreClient;
        private readonly IEventDispatcher _eventDispatcher;
        private readonly IContext _context;
        private readonly IMessageContextRegistry _messageContextRegistry;

        public AggregateStore
        (
            IClockService clockService,
            CustomEventStoreClient eventStoreClient,
            IEventDispatcher eventDispatcher, 
            IContext context,
            IMessageContextRegistry messageContextRegistry
        )
        {
            _clockService = clockService;
            _eventStoreClient = eventStoreClient;
            _eventDispatcher = eventDispatcher;
            _context = context;
            _messageContextRegistry = messageContextRegistry;
        }

        public async Task SaveAndPublishAsync
        (
            TAggregate aggregate,
            CancellationToken cancellationToken = default
        )
        {
            if (aggregate is null) throw new ArgumentNullException(nameof(aggregate));

            IReadOnlyList<IBaseEvent> events = await SaveAsync(aggregate, cancellationToken);

            try
            {
                await PublishAsync(events, cancellationToken);
            }
            finally
            {
                aggregate.Clear();
            }
        }

        public async Task<IReadOnlyList<IBaseEvent>> SaveAsync
        (
            TAggregate aggregate,
            CancellationToken cancellationToken = default // TODO  - MessageId, CorrelationId
        )
        {
            if (aggregate is null) throw new ArgumentNullException(nameof(aggregate));

            foreach (IBaseEvent @event in aggregate.Events)
            {
                _messageContextRegistry.Set
                (
                    @event,
                    new MessageContext(SequentialGuid.Create(), _context)
                );
            }
            
            await _eventStoreClient.AppendToStreamAsync
            (
                GetStreamName(aggregate.Id),
                aggregate.Version,
                aggregate.Events,
                cancellationToken
            );

            return aggregate.Events;
        }

        public async Task<TAggregate> LoadAsync
        (
            EntityId aggregateId,
            CancellationToken cancellationToken = default
        )
        {
            IReadOnlyList<IBaseEvent> events = await _eventStoreClient
                .ReadStreamForwardAsync<IBaseEvent>(GetStreamName(aggregateId), cancellationToken);

            if (events.Count is 0) return default;

            TAggregate aggregate = new();
            aggregate.Load(events);
            
            IReadOnlyList<IBaseEvent> processedEvents = events
                .Where(e => e.Metadata.CausationId == _context.RequestId).ToList();

            if (!processedEvents.Any()) return aggregate;
            
            await PublishAsync(processedEvents, cancellationToken); 
            aggregate.Restore();

            return aggregate;
        }
        
        public async Task PublishAsync
        (
            IEnumerable<IBaseEvent> events,
            CancellationToken cancellationToken = default
        )
        {
            // https://stackoverflow.com/questions/59320296/how-to-add-mediatr-publishstrategy-to-existing-project
            foreach (IBaseEvent @event in events)
                await _eventDispatcher.PublishAsync(@event, NotificationDispatchStrategy.SyncStopOnException, cancellationToken);
        }

        public static string GetStreamName(EntityId aggregateId) => $"aggregate/{typeof(TAggregate).Name}/{aggregateId}";
    }
}