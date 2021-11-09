using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Serilog;
using EventStore.ClientAPI;

using VShop.SharedKernel.EventStore.Extensions;
using VShop.SharedKernel.Domain.ValueObjects;
using VShop.SharedKernel.EventSourcing.Aggregates;
using VShop.SharedKernel.EventSourcing.Repositories;
using VShop.SharedKernel.Infrastructure.Extensions;
using VShop.SharedKernel.Infrastructure.Messaging.Events;
using VShop.SharedKernel.Infrastructure.Messaging.Events.Publishing;

using ILogger = Serilog.ILogger;

namespace VShop.SharedKernel.EventStore.Repositories
{
    public class EventStoreAggregateRepository<TA, TKey> : IAggregateRepository<TA, TKey>
        where TKey : ValueObject
        where TA : AggregateRoot<TKey>
    {
        private readonly IEventStoreConnection _eventStoreConnection;
        private readonly Publisher _publisher;

        private static readonly ILogger Logger = Log.ForContext<EventStoreAggregateRepository<TA, TKey>>();

        public EventStoreAggregateRepository
        (
            IEventStoreConnection eventStoreConnection,
            Publisher publisher
        )
        {
            _eventStoreConnection = eventStoreConnection;
            _publisher = publisher;
        }

        public async Task SaveAsync(TA aggregate)
        {
            if (aggregate is null)
                throw new ArgumentNullException(nameof(aggregate));

            string streamName = GetAggregateStreamName(aggregate.Id);

            await _eventStoreConnection.AppendToStreamAsync
            (
                streamName,
                aggregate.Version,
                aggregate.GetAllEvents().ToArray()
            );

            try
            {
                // https://stackoverflow.com/questions/59320296/how-to-add-mediatr-publishstrategy-to-existing-project
                foreach (IDomainEvent domainEvent in aggregate.GetOutgoingDomainEvents())
                    await _publisher.Publish(domainEvent, PublishStrategy.SyncStopOnException);
            }
            finally
            {
                aggregate.ClearAllEvents();
            }
        }
        
        public async Task<bool> ExistsAsync(TKey aggregateId)
        {
            string streamName = GetAggregateStreamName(aggregateId);
            EventReadResult result = await _eventStoreConnection.ReadEventAsync(streamName, 1, false);
            
            return result.Status is not EventReadStatus.NoStream;
        }
        
        public async Task<TA> LoadAsync(TKey aggregateId, Guid? messageId = null, Guid? correlationId = null)
        {
            string streamName = GetAggregateStreamName(aggregateId);
            List<IEvent> events = await _eventStoreConnection.ReadStreamEventsForwardAsync<IEvent>(streamName);

            if (events.Count is 0) return default;
                
            TA aggregate = (TA)Activator.CreateInstance(typeof(TA), true);
            if (aggregate is null)
                throw new Exception($"Couldn't resolve {nameof(TA)} instance.");
            
            if (messageId is not null) aggregate.MessageId = messageId.Value;
            if (correlationId is not null) aggregate.CorrelationId = correlationId.Value;
            
            aggregate?.Load(events);

            return aggregate;
        }

        private string GetAggregateStreamName(TKey aggregateId)
            => $"{_eventStoreConnection.ConnectionName}/aggregate/{typeof(TA).Name}/{aggregateId}".ToSnakeCase();
    }
}