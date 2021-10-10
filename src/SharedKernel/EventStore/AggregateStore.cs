using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using EventStore.ClientAPI;

using VShop.SharedKernel.EventSourcing;
using VShop.SharedKernel.Infrastructure.Domain;

namespace VShop.SharedKernel.EventStore
{
    public class AggregateStore : IAggregateStore
    {
        private readonly IEventStoreConnection _connection;

        public AggregateStore(IEventStoreConnection connection) => _connection = connection;
        
        public async Task SaveAsync<T>(T aggregate) 
            where T : AggregateRoot
        {
            if (aggregate == null)
                throw new ArgumentNullException(nameof(aggregate));

            string streamName = GetStreamName<T>(aggregate.Id);
            object[] events = aggregate.GetChanges().ToArray();

            await _connection.AppendEvents(streamName, aggregate.Version, events);

            aggregate.ClearChanges();
        }
        
        public async Task<bool> ExistsAsync<T>(EntityId aggregateId)
            where T : AggregateRoot
        {
            string streamName = GetStreamName<T>(aggregateId);
            EventReadResult result = await _connection.ReadEventAsync(streamName, 1, false);
            
            return result.Status != EventReadStatus.NoStream;
        }
        
        public async Task<T> LoadAsync<T>(EntityId aggregateId)
            where T : AggregateRoot
        {
            const int maxSliceSize = 4096;
            
            T aggregate = (T)Activator.CreateInstance(typeof(T), true); 
            string streamName = GetStreamName<T>(aggregateId);

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

        private static string GetStreamName<T>(EntityId aggregateId)
            where T : AggregateRoot
            => $"{typeof(T).Name}-{aggregateId}";
    }
}