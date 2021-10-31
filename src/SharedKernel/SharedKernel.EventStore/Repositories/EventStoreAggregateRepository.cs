using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using MediatR;
using EventStore.ClientAPI;

using VShop.SharedKernel.EventSourcing;
using VShop.SharedKernel.EventStore.Helpers;
using VShop.SharedKernel.EventStore.Extensions;
using VShop.SharedKernel.EventStore.Repositories.Contracts;
using VShop.SharedKernel.Domain.ValueObjects;
using VShop.SharedKernel.Infrastructure.Extensions;
using VShop.SharedKernel.Infrastructure.Messaging;

namespace VShop.SharedKernel.EventStore.Repositories
{
    public class EventStoreAggregateRepository<TA, TKey> : IEventStoreAggregateRepository<TA, TKey>
        where TKey : ValueObject
        where TA : AggregateRoot<TKey>
    {
        private readonly IEventStoreConnection _esConnection;
        private readonly IMediator _mediator;

        public EventStoreAggregateRepository
        (
            IEventStoreConnection esConnection,
            IMediator mediator
        )
        {
            _esConnection = esConnection;
            _mediator = mediator;
        }
        
        public async Task SaveAsync(TA aggregate)
        {
            if (aggregate is null)
                throw new ArgumentNullException(nameof(aggregate));

            string streamName = GetStreamName(aggregate.Id);
            IDomainEvent[] events = aggregate.GetChanges().ToArray();

            await _esConnection.AppendToStreamAsync
            (
                streamName,
                aggregate.Version,
                EventStoreHelper.PrepareEventData(messages: events)
            );

            aggregate.ClearChanges();
            
            // TODO - should I change publish strategy? Should I omit this code?
            // error handling - I don't think there is a need for additional error handling. Command decorator will wrap exceptions.
            // https://github.com/jbogard/MediatR/blob/master/samples/MediatR.Examples.PublishStrategies/PublishStrategy.cs
            // https://stackoverflow.com/questions/59320296/how-to-add-mediatr-publishstrategy-to-existing-project
            foreach (IDomainEvent @event in events)
                await _mediator.Publish(@event);
        }
        
        public async Task<bool> ExistsAsync(TKey aggregateId)
        {
            string streamName = GetStreamName(aggregateId);
            EventReadResult result = await _esConnection.ReadEventAsync(streamName, 1, false);
            
            return result.Status != EventReadStatus.NoStream;
        }
        
        public async Task<TA> LoadAsync(TKey aggregateId)
        {
            List<IDomainEvent> events = await _esConnection.ReadStreamEventsForwardAsync<IDomainEvent>(GetStreamName(aggregateId));

            if (events.Count == 0) return default;
            
            TA aggregate = (TA)Activator.CreateInstance(typeof(TA), true); 
            aggregate?.Load(events);

            return aggregate;
        }

        private string GetStreamName(TKey aggregateId)
            => $"{_esConnection.ConnectionName}/{typeof(TA).Name}/{aggregateId}".ToSnakeCase();
    }
}