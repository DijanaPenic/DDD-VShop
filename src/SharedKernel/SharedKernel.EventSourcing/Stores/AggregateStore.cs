﻿using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using EventStore.Client;

using VShop.SharedKernel.Domain.ValueObjects;
using VShop.SharedKernel.EventStoreDb.Extensions;
using VShop.SharedKernel.Infrastructure.Types;
using VShop.SharedKernel.Infrastructure.Services.Contracts;
using VShop.SharedKernel.EventSourcing.Aggregates;
using VShop.SharedKernel.EventSourcing.Stores.Contracts;
using VShop.SharedKernel.Infrastructure.Dispatchers;
using VShop.SharedKernel.Infrastructure.Events.Contracts;
using VShop.SharedKernel.Infrastructure.Messaging;

namespace VShop.SharedKernel.EventSourcing.Stores
{
    public class AggregateStore<TAggregate> : IAggregateStore<TAggregate> where TAggregate : AggregateRoot, new()
    {
        private readonly IClockService _clockService;
        private readonly EventStoreClient _eventStoreClient;
        private readonly IEventDispatcher _eventDispatcher;

        public AggregateStore
        (
            IClockService clockService,
            EventStoreClient eventStoreClient,
            IEventDispatcher eventDispatcher
        )
        {
            _clockService = clockService;
            _eventStoreClient = eventStoreClient;
            _eventDispatcher = eventDispatcher;
        }

        public async Task SaveAndPublishAsync
        (
            TAggregate aggregate,
            Guid messageId,
            Guid correlationId,
            CancellationToken cancellationToken = default
        )
        {
            if (aggregate is null) throw new ArgumentNullException(nameof(aggregate));

            IList<IBaseEvent> events = await SaveAsync(aggregate, messageId, correlationId, cancellationToken);

            try
            {
                await PublishAsync(events, cancellationToken);
            }
            finally
            {
                aggregate.Clear();
            }
        }

        public async Task<IList<IBaseEvent>> SaveAsync
        (
            TAggregate aggregate,
            Guid messageId,
            Guid correlationId,
            CancellationToken cancellationToken = default
        )
        {
            if (aggregate is null) throw new ArgumentNullException(nameof(aggregate));

            IList<IBaseEvent> events = aggregate.Events.Select(@event =>
            {
                @event.Metadata = new MessageMetadata
                (
                    SequentialGuid.Create(),
                    messageId,
                    correlationId,
                    _clockService.Now
                );
                return @event;
            }).ToList();
            
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
            IReadOnlyList<IBaseEvent> events = await _eventStoreClient
                .ReadStreamForwardAsync<IBaseEvent>(GetStreamName(aggregateId), cancellationToken);

            if (events.Count is 0) return default;

            TAggregate aggregate = new();
            aggregate.Load(events);
            
            IList<IBaseEvent> processedEvents = events
                .Where(e => e.Metadata.CausationId == messageId).ToList();

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