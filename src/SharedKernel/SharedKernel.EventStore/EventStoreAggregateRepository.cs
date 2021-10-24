using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using MediatR;
using EventStore.ClientAPI;

using VShop.SharedKernel.Domain;
using VShop.SharedKernel.EventSourcing;
using VShop.SharedKernel.Infrastructure.Extensions;

namespace VShop.SharedKernel.EventStore
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
            if (aggregate == null)
                throw new ArgumentNullException(nameof(aggregate));

            string streamName = GetStreamName(aggregate.Id);
            IDomainEvent[] events = aggregate.GetChanges().ToArray();

            await _esConnection.AppendEvents(streamName, aggregate.Version, events);

            aggregate.ClearChanges();
            
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
            const int maxSliceSize = 4096;
            
            string streamName = GetStreamName(aggregateId);

            long position = 0L;
            bool endOfStream;
            List<IDomainEvent> events = new();

            do
            {
                StreamEventsSlice slice = await _esConnection.ReadStreamEventsForwardAsync
                (
                    streamName, 
                    position, 
                    maxSliceSize, 
                    false
                );
                
                position = slice.NextEventNumber;
                endOfStream = slice.IsEndOfStream;

                events.AddRange(slice.Events.Select(resolvedEvent => resolvedEvent.DeserializeData()));
            } while (!endOfStream);

            if (events.Count == 0) return default;
            
            TA aggregate = (TA)Activator.CreateInstance(typeof(TA), true); 
            aggregate?.Load(events);

            return aggregate;
        }

        private string GetStreamName(TKey aggregateId)
            => $"{_esConnection.ConnectionName}/{typeof(TA).Name}/{aggregateId}".ToSnakeCase();
    }
}