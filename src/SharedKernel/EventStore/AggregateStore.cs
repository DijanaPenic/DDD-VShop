using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using EventStore.ClientAPI;

using VShop.SharedKernel.EventSourcing;
using VShop.SharedKernel.Infrastructure.Domain;

namespace VShop.SharedKernel.EventStore
{
    public class AggregateStore<TA, TKey> : IAggregateStore<TA, TKey>
        where TKey : ValueObject
        where TA : AggregateRoot<TKey>
    {
        private readonly IEventStoreConnection _connection;

        public AggregateStore(IEventStoreConnection connection) => _connection = connection;
        
        public async Task SaveAsync(TA aggregate)
        {
            if (aggregate == null)
                throw new ArgumentNullException(nameof(aggregate));

            string streamName = GetStreamName(aggregate.Id);
            object[] events = aggregate.GetChanges().ToArray();

            await _connection.AppendEvents(streamName, aggregate.Version, events);

            aggregate.ClearChanges();
        }
        
        public async Task<bool> ExistsAsync(TKey aggregateId)
        {
            string streamName = GetStreamName(aggregateId);
            EventReadResult result = await _connection.ReadEventAsync(streamName, 1, false);
            
            return result.Status != EventReadStatus.NoStream;
        }
        
        public async Task<TA> LoadAsync(TKey aggregateId)
        {
            const int maxSliceSize = 4096;
            
            TA aggregate = (TA)Activator.CreateInstance(typeof(TA), true); 
            string streamName = GetStreamName(aggregateId);

            long position = 0L;
            bool endOfStream;
            List<object> events = new();

            do
            {
                StreamEventsSlice slice = await _connection.ReadStreamEventsForwardAsync
                (
                    streamName, 
                    position, 
                    maxSliceSize, 
                    false
                );
                
                position = slice.NextEventNumber;
                endOfStream = slice.IsEndOfStream;

                events.AddRange(slice.Events.Select(resolvedEvent => resolvedEvent.Deserialize()));
            } while (!endOfStream);
            
            aggregate?.Load(events);

            return aggregate;
        }

        private static string GetStreamName(TKey aggregateId)
            => $"{typeof(TA).Name}-{aggregateId}";
    }
}