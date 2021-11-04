using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using EventStore.ClientAPI;

using VShop.SharedKernel.EventStore.Extensions;
using VShop.SharedKernel.Infrastructure.Helpers;
using VShop.SharedKernel.Infrastructure.Extensions;
using VShop.SharedKernel.EventSourcing.Repositories.Contracts;

namespace VShop.SharedKernel.EventStore.Repositories
{
    public class EventStoreCheckpointRepository : ICheckpointRepository
    {       
        private readonly IEventStoreConnection _eventStoreConnection;
        private readonly string _checkpointStreamName;

        public EventStoreCheckpointRepository
        (
            IEventStoreConnection eventStoreConnection,
            string subscriptionName
        )
        {
            _eventStoreConnection = eventStoreConnection;
            _checkpointStreamName = $"{eventStoreConnection.ConnectionName}/checkpoint/{subscriptionName}".ToSnakeCase();
        }

        public async Task<long?> GetCheckpointAsync()
        {
            StreamEventsSlice slice = await _eventStoreConnection.ReadStreamEventsBackwardAsync(_checkpointStreamName, -1, 1, false);
            ResolvedEvent eventData = slice.Events.FirstOrDefault();

            if (!eventData.Equals(default(ResolvedEvent))) return eventData.DeserializeData<Checkpoint>()?.Position;
            
            await StoreCheckpointAsync(AllCheckpoint.AllStart?.CommitPosition);
            await SetStreamMaxCountAsync();
                
            return null;
        }

        public Task StoreCheckpointAsync(long? checkpoint)
        {
            Checkpoint checkpointData = new() { Position = checkpoint };

            EventData @event = new
            (
                SequentialGuid.Create(),
                "$checkpoint",
                true,
                Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(checkpointData)),
                null
            );

            return _eventStoreConnection.AppendToStreamAsync
            (
                _checkpointStreamName,
                ExpectedVersion.Any,
                @event
            );
        }

        private async Task SetStreamMaxCountAsync()
        {
            StreamMetadataResult metadata = await _eventStoreConnection.GetStreamMetadataAsync(_checkpointStreamName);

            if (!metadata.StreamMetadata.MaxCount.HasValue)
                await _eventStoreConnection.SetStreamMetadataAsync
                (
                    _checkpointStreamName, 
                    ExpectedVersion.Any,
                    StreamMetadata.Create(1)
                );
        }

        private class Checkpoint
        {
            public long? Position { get; init; }
        }
    }
}