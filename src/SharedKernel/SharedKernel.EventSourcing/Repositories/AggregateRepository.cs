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
using VShop.SharedKernel.EventSourcing.Repositories.Contracts;
using VShop.SharedKernel.Infrastructure.Extensions;
using VShop.SharedKernel.Infrastructure.Services.Contracts;

namespace VShop.SharedKernel.EventSourcing.Repositories
{
    public class AggregateRepository<TAggregate, TKey> : IAggregateRepository<TAggregate, TKey>
        where TAggregate : AggregateRoot<TKey>, new()
        where TKey : ValueObject
    {
        private readonly IClockService _clockService;
        private readonly EventStoreClient _eventStoreClient;
        private readonly IEventBus _eventBus;

        public AggregateRepository
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
                // https://stackoverflow.com/questions/59320296/how-to-add-mediatr-publishstrategy-to-existing-project
                foreach (IDomainEvent domainEvent in aggregate.GetDomainEvents())
                    await _eventBus.Publish(domainEvent, EventPublishStrategy.SyncStopOnException, cancellationToken);
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
            TKey aggregateId,
            Guid? messageId = default,
            Guid? correlationId = default,
            CancellationToken cancellationToken = default
        )
        {
            string streamName = GetStreamName(aggregateId);
            
            IList<IBaseEvent> events = await _eventStoreClient.ReadStreamForwardAsync<IBaseEvent>
            (
                streamName,
                StreamPosition.Start,
                cancellationToken
            );

            if (events.Count is 0) return default;

            TAggregate aggregate = new();
            aggregate.Load(events, messageId, correlationId);

            return aggregate;
        }
        
        private async Task AppendMessagesToStreamAsync(TAggregate aggregate, CancellationToken cancellationToken = default)
        {
            if (aggregate is null)
                throw new ArgumentNullException(nameof(aggregate));

            string streamName = GetStreamName(aggregate.Id);

            await _eventStoreClient.AppendToStreamAsync
            (
                _clockService,
                streamName,
                aggregate.Version,
                aggregate.GetAllEvents(),
                cancellationToken
            );
        }


        private string GetStreamName(TKey aggregateId)
            => $"{_eventStoreClient.ConnectionName}/aggregate/{typeof(TAggregate).Name}/{aggregateId}".ToSnakeCase();
    }
}