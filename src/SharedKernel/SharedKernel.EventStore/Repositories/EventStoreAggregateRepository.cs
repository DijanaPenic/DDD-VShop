using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using EventStore.ClientAPI;

using VShop.SharedKernel.EventSourcing;
using VShop.SharedKernel.EventSourcing.Contracts;
using VShop.SharedKernel.EventStore.Helpers;
using VShop.SharedKernel.EventStore.Extensions;
using VShop.SharedKernel.Domain.ValueObjects;
using VShop.SharedKernel.Infrastructure.Extensions;
using VShop.SharedKernel.Infrastructure.Messaging;

namespace VShop.SharedKernel.EventStore.Repositories
{
    public class EventStoreAggregateRepository<TA, TKey> : IAggregateRepository<TA, TKey>
        where TKey : ValueObject
        where TA : AggregateRoot<TKey>
    {
        private readonly IEventStoreConnection _eventStoreConnection;
        private readonly Publisher _publisher;

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
            
            IDomainEvent[] domainEvents = aggregate.GetDomainEvents().ToArray();
            IIntegrationEvent[] integrationEvents = aggregate.GetIntegrationEvents().ToArray();
            IMessage[] allEvents = domainEvents.Concat(integrationEvents.Cast<IMessage>()).ToArray();

            await _eventStoreConnection.AppendToStreamAsync
            (
                streamName,
                aggregate.Version,
                EventStoreHelper.PrepareEventData(allEvents)
            );

            aggregate.ClearEvents();
            
            // TODO - error handling - I don't think there is a need for additional error handling. Command decorator will wrap exceptions.
            // https://stackoverflow.com/questions/59320296/how-to-add-mediatr-publishstrategy-to-existing-project
            foreach (IDomainEvent domainEvent in domainEvents)
                await _publisher.Publish(domainEvent, PublishStrategy.SyncStopOnException);
        }
        
        public async Task<bool> ExistsAsync(TKey aggregateId)
        {
            string streamName = GetAggregateStreamName(aggregateId);
            EventReadResult result = await _eventStoreConnection.ReadEventAsync(streamName, 1, false);
            
            return result.Status != EventReadStatus.NoStream;
        }
        
        public async Task<TA> LoadAsync(TKey aggregateId)
        {
            string streamName = GetAggregateStreamName(aggregateId);
            List<IMessage> events = await _eventStoreConnection.ReadStreamEventsForwardAsync<IMessage>(streamName);

            if (events.Count == 0) return default;
            
            TA aggregate = (TA)Activator.CreateInstance(typeof(TA), true); 
            aggregate?.Load(events);

            return aggregate;
        }

        private string GetAggregateStreamName(TKey aggregateId)
            => $"{_eventStoreConnection.ConnectionName}/aggregate/{typeof(TA).Name}/{aggregateId}".ToSnakeCase();
    }
}