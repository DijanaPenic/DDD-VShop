using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using EventStore.Client;

using VShop.SharedKernel.Messaging;
using VShop.SharedKernel.Messaging.Events;
using VShop.SharedKernel.Messaging.Events.Publishing;
using VShop.SharedKernel.Messaging.Events.Publishing.Contracts;
using VShop.SharedKernel.Domain.ValueObjects;
using VShop.SharedKernel.EventStoreDb.Extensions;
using VShop.SharedKernel.Infrastructure.Helpers;
using VShop.SharedKernel.Infrastructure.Services.Contracts;
using VShop.SharedKernel.EventSourcing.Aggregates;
using VShop.SharedKernel.EventSourcing.Stores.Contracts;

namespace VShop.SharedKernel.EventSourcing.Stores
{
    public class AggregateStore<TAggregate> : IAggregateStore<TAggregate> where TAggregate : AggregateRoot, new()
    {
        private readonly IClockService _clockService;
        private readonly EventStoreClient _eventStoreClient;
        private readonly IEventBus _eventBus;

        public AggregateStore
        (
            IClockService clockService,
            EventStoreClient eventStoreClient,
            IEventBus eventBus
        )
        {
            _clockService = clockService;
            _eventStoreClient = eventStoreClient;
            _eventBus = eventBus;
        }

        public async Task SaveAndPublishAsync
        (
            TAggregate aggregate,
            Guid messageId,
            Guid correlationId,
            CancellationToken cancellationToken = default
        )
        {
            if (aggregate is null)
                throw new ArgumentNullException(nameof(aggregate));

            IList<IdentifiedEvent<IBaseEvent>> events = await SaveAsync
            (
                aggregate,
                messageId,
                correlationId,
                cancellationToken
            );

            try
            {
                await PublishAsync(events, cancellationToken);
            }
            finally
            {
                aggregate.Clear();
            }
        }

        // TODO - can I merge Publish and Save?
        public async Task<IList<IdentifiedEvent<IBaseEvent>>> SaveAsync
        (
            TAggregate aggregate,
            Guid messageId,
            Guid correlationId,
            CancellationToken cancellationToken = default
        )
        {
            if (aggregate is null)
                throw new ArgumentNullException(nameof(aggregate));

            IList<IdentifiedEvent<IBaseEvent>> events = aggregate.Events
                .Select(@event => new IdentifiedEvent<IBaseEvent>
                (
                    @event,
                    new MessageMetadata
                    (
                        SequentialGuid.Create(),
                        messageId,
                        correlationId,
                        _clockService.Now
                    )
                )).ToList();
            
            await _eventStoreClient.AppendToStreamAsync
            (
                GetStreamName(aggregate.Id),
                aggregate.Version,
                events,
                cancellationToken
            );

            return events;
        }

        public async Task<TAggregate> LoadAsync
        (
            EntityId aggregateId,
            Guid messageId,
            CancellationToken cancellationToken = default
        )
        {
            IReadOnlyList<IdentifiedEvent<IBaseEvent>> events = (await _eventStoreClient
                .ReadStreamForwardAsync<IBaseEvent>(GetStreamName(aggregateId), cancellationToken))
                .Select(e => new IdentifiedEvent<IBaseEvent>(e))
                .ToList();

            if (events.Count is 0) return default;

            TAggregate aggregate = new();
            aggregate.Load(events);
            
            IList<IdentifiedEvent<IBaseEvent>> processedEvents = events
                .Where(e => e.Metadata.CausationId == messageId).ToList();

            if (!processedEvents.Any()) return aggregate;
            
            await PublishAsync(processedEvents, cancellationToken); 
            aggregate.Restore();

            return aggregate;
        }
        
        public async Task PublishAsync
        (
            IEnumerable<IIdentifiedEvent<IBaseEvent>> events,
            CancellationToken cancellationToken = default
        )
        {
            // https://stackoverflow.com/questions/59320296/how-to-add-mediatr-publishstrategy-to-existing-project
            foreach (IIdentifiedEvent<IBaseEvent> @event in events)
                await _eventBus.Publish(@event, EventPublishStrategy.SyncStopOnException, cancellationToken);
        }

        public static string GetStreamName(EntityId aggregateId) => $"aggregate/{typeof(TAggregate).Name}/{aggregateId}";
    }
}