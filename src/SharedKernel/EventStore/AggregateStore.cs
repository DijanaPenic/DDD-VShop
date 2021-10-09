using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using EventStore.ClientAPI;

using VShop.SharedKernel.EventSourcing;

namespace VShop.SharedKernel.EventStore
{
    public class AggregateStore : IAggregateStore
    {
        private readonly IEventStoreConnection _connection;

        public AggregateStore(IEventStoreConnection connection) => _connection = connection;

        public Task UpdateAsync<T>(long version, AggregateState<T>.Result update)
            where T : class, IAggregateState<T>, new()
            => _connection.AppendEvents(update.State.StreamName, version, update.Events.ToArray());
        
        public Task CreateAsync<T>(AggregateState<T>.Result create)
            where T : class, IAggregateState<T>, new()
            => _connection.AppendEvents(create.State.StreamName, -1, create.Events.ToArray()); // -1 version -> the stream should not exist when appending.

        public Task<T> LoadAsync<T>(Guid id) 
            where T : IAggregateState<T>, new()
            => LoadAsync<T>(id, (aggregateState, @event) => aggregateState.When(aggregateState, @event));

        private async Task<T> LoadAsync<T>(Guid id, Func<T, object, T> when)
            where T : IAggregateState<T>, new()
        {
            const int maxSliceSize = 4096;

            T aggregateState = new T();
            string streamName = aggregateState.GetStreamName(id);

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

            return events.Aggregate(aggregateState, when);
        }

    }
}