using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using EventStore.Client;

using VShop.SharedKernel.Messaging.Events;
using VShop.SharedKernel.Messaging.Events.Publishing;
using VShop.SharedKernel.Messaging.Events.Publishing.Contracts;
using VShop.SharedKernel.Domain.ValueObjects;
using VShop.SharedKernel.EventStoreDb.Extensions;
using VShop.SharedKernel.EventSourcing.Aggregates;
using VShop.SharedKernel.EventSourcing.Stores.Contracts;
using VShop.SharedKernel.Infrastructure.Extensions;
using VShop.SharedKernel.Infrastructure.Services.Contracts;

namespace VShop.SharedKernel.EventSourcing.Stores
{
    public class AggregateStore<TAggregate> : IAggregateStore<TAggregate> where TAggregate : AggregateRoot
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
            await AppendMessagesToStreamAsync(aggregate, cancellationToken);

            try
            {
                await PublishAsync(aggregate, cancellationToken);
            }
            finally
            {
                aggregate.Clear();
            }
        }
        
        public async Task SaveAsync(TAggregate aggregate, CancellationToken cancellationToken = default)
        {
            await AppendMessagesToStreamAsync(aggregate, cancellationToken);
            aggregate.Clear();
        }

        public async Task<TAggregate> LoadAsync
        (
            EntityId aggregateId,
            Guid? causationId = default,
            Guid? correlationId = default,
            CancellationToken cancellationToken = default
        )
        {
            IReadOnlyList<IBaseEvent> events = await _eventStoreClient.ReadStreamForwardAsync<IBaseEvent>
            (
                GetStreamName(aggregateId),
                StreamPosition.Start,
                cancellationToken
            );

            if (events.Count is 0) return default;

            // TODO - potentially use private constructor.
            TAggregate aggregate = (TAggregate)Activator.CreateInstance
            (
                typeof(TAggregate),
                causationId ?? Guid.Empty, correlationId ?? Guid.Empty
            );
            
            if (aggregate is null) throw new Exception("Aggregate instance creation failed.");
            
            aggregate.Load(events);

            return aggregate;
        }
        
        private async Task PublishAsync(TAggregate aggregate, CancellationToken cancellationToken = default)
        {
            // https://stackoverflow.com/questions/59320296/how-to-add-mediatr-publishstrategy-to-existing-project
            foreach (IDomainEvent domainEvent in aggregate.GetOutboxMessages<IDomainEvent>())
                await _eventBus.Publish(domainEvent, EventPublishStrategy.SyncStopOnException, cancellationToken);
        }
        
        private async Task AppendMessagesToStreamAsync(TAggregate aggregate, CancellationToken cancellationToken = default)
        {
            if (aggregate is null)
                throw new ArgumentNullException(nameof(aggregate));

            string streamName = GetStreamName(aggregate.Id);

            await _eventStoreClient.AppendToStreamAsync
            (
                streamName,
                aggregate.Version,
                aggregate.GetOutboxMessages(),
                _clockService.Now,
                cancellationToken
            );
        }
        
        private string GetStreamName(EntityId aggregateId)
            => $"{_eventStoreClient.ConnectionName}/aggregate/{typeof(TAggregate).Name}/{aggregateId}".ToSnakeCase();
    }
}