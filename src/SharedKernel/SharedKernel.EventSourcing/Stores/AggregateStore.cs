﻿using System;
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
using VShop.SharedKernel.EventSourcing.Aggregates;
using VShop.SharedKernel.EventSourcing.Stores.Contracts;
using VShop.SharedKernel.Infrastructure.Services.Contracts;

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

        public async Task SaveAndPublishAsync(TAggregate aggregate, CancellationToken cancellationToken = default)
        {
            if (aggregate is null)
                throw new ArgumentNullException(nameof(aggregate));

            await SaveAsync(aggregate, cancellationToken);

            try
            {
                // https://stackoverflow.com/questions/59320296/how-to-add-mediatr-publishstrategy-to-existing-project
                foreach (IIdentifiedEvent<IDomainEvent> domainEvent in aggregate.DomainEvents)
                    await _eventBus.Publish(domainEvent, EventPublishStrategy.SyncStopOnException, cancellationToken);
            }
            finally
            {
                aggregate.Clear();
            }
        }
        
        public async Task SaveAsync(TAggregate aggregate, CancellationToken cancellationToken = default)
        {
            if (aggregate is null)
                throw new ArgumentNullException(nameof(aggregate));

            await _eventStoreClient.AppendToStreamAsync
            (
                GetStreamName(aggregate.Id),
                aggregate.Version,
                aggregate.Events,
                _clockService.Now,
                cancellationToken
            );
        }

        public async Task<TAggregate> LoadAsync
        (
            EntityId aggregateId,
            Guid causationId,
            Guid correlationId,
            CancellationToken cancellationToken = default
        )
        {
            IReadOnlyList<IIdentifiedMessage<IBaseEvent>> events = await _eventStoreClient
                .ReadStreamForwardAsync<IBaseEvent>
                (
                    GetStreamName(aggregateId),
                    cancellationToken
                );

            if (events.Count is 0) return default;

            TAggregate aggregate = new();
            aggregate.Load
            (
                events.Select(e => new IdentifiedEvent<IBaseEvent>(e)),
                causationId,
                correlationId
            );

            return aggregate;
        }

        public static string GetStreamName(EntityId aggregateId) => $"aggregate/{typeof(TAggregate).Name}/{aggregateId}";
    }
}