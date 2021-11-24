using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Serilog;
using EventStore.Client;

using VShop.SharedKernel.Messaging.Events;
using VShop.SharedKernel.Messaging.Events.Publishing;
using VShop.SharedKernel.Messaging.Events.Publishing.Contracts;
using VShop.SharedKernel.Domain.ValueObjects;
using VShop.SharedKernel.EventStoreDb.Extensions;
using VShop.SharedKernel.EventSourcing.Aggregates;
using VShop.SharedKernel.EventSourcing.Repositories;
using VShop.SharedKernel.Infrastructure.Extensions;

using ILogger = Serilog.ILogger;

namespace VShop.SharedKernel.EventStoreDb.Repositories
{
    public class EventStoreAggregateRepository<TAggregate, TKey> : IAggregateRepository<TAggregate, TKey>
        where TKey : ValueObject
        where TAggregate : AggregateRoot<TKey>
    {
        private readonly EventStoreClient _eventStoreClient;
        private readonly IEventBus _eventBus;

        private static readonly ILogger Logger = Log.ForContext<EventStoreAggregateRepository<TAggregate, TKey>>();

        public EventStoreAggregateRepository
        (
            EventStoreClient eventStoreClient,
            IEventBus eventBus
        )
        {
            _eventStoreClient = eventStoreClient;
            _eventBus = eventBus;
        }

        public async Task SaveAsync(TAggregate aggregate, CancellationToken cancellationToken = default)
        {
            if (aggregate is null)
                throw new ArgumentNullException(nameof(aggregate));

            string streamName = GetStreamName(aggregate.Id);

            await _eventStoreClient.AppendToStreamAsync
            (
                streamName,
                aggregate.Version,
                aggregate.GetAllEvents(),
                cancellationToken
            );

            try
            {
                // https://stackoverflow.com/questions/59320296/how-to-add-mediatr-publishstrategy-to-existing-project
                foreach (IDomainEvent domainEvent in aggregate.GetOutgoingDomainEvents())
                    await _eventBus.Publish(domainEvent, EventPublishStrategy.SyncStopOnException, cancellationToken);
            }
            finally
            {
                aggregate.Clear();
            }
        }

        public async Task<TAggregate> LoadAsync
        (
            TKey aggregateId,
            Guid? causationId = null,
            Guid? correlationId = null,
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
                
            TAggregate aggregate = (TAggregate)Activator.CreateInstance(typeof(TAggregate), true);
            if (aggregate is null)
                throw new Exception($"Couldn't resolve {nameof(aggregate)} instance.");
            
            if (causationId is not null) aggregate.CausationId = causationId.Value;
            if (correlationId is not null) aggregate.CorrelationId = correlationId.Value;
            
            aggregate.Load(events);

            return aggregate;
        }

        private string GetStreamName(TKey aggregateId)
            => $"{_eventStoreClient.ConnectionName}/aggregate/{typeof(TAggregate).Name}/{aggregateId}".ToSnakeCase();
    }
}