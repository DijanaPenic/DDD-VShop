using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using EventStore.ClientAPI;

using VShop.SharedKernel.Infrastructure.Helpers;

namespace VShop.SharedKernel.EventStore
{
    public class EventStoreCheckpointRepository : IEventStoreCheckpointRepository
    {
        private const string CheckpointStreamPrefix = "checkpoint:";
        
        private readonly IEventStoreConnection _esConnection;
        private readonly string _esSubscriptionName;

        public EventStoreCheckpointRepository
        (
            IEventStoreConnection esConnection,
            string esSubscriptionName
        )
        {
            _esConnection = esConnection;
            _esSubscriptionName = CheckpointStreamPrefix + esSubscriptionName;
        }

        public async Task<long?> GetCheckpointAsync()
        {
            StreamEventsSlice slice = await _esConnection.ReadStreamEventsBackwardAsync(_esSubscriptionName, -1, 1, false);
            ResolvedEvent eventData = slice.Events.FirstOrDefault();

            if (!eventData.Equals(default(ResolvedEvent))) return eventData.DeserializeData<Checkpoint>()?.Position;
            
            await StoreCheckpointAsync(AllCheckpoint.AllStart?.CommitPosition);
            await SetStreamMaxCountAsync();
                
            return null;
        }

        public Task StoreCheckpointAsync(long? checkpoint)
        {
            Checkpoint @event = new() { Position = checkpoint };

            EventData preparedEvent = new
            (
                GuidHelper.NewSequentialGuid(),
                "$checkpoint",
                true,
                Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(@event)),
                null
            );

            return _esConnection.AppendToStreamAsync
            (
                _esSubscriptionName,
                ExpectedVersion.Any,
                preparedEvent
            );
        }

        private async Task SetStreamMaxCountAsync()
        {
            StreamMetadataResult metadata = await _esConnection.GetStreamMetadataAsync(_esSubscriptionName);

            if (!metadata.StreamMetadata.MaxCount.HasValue)
                await _esConnection.SetStreamMetadataAsync
                (
                    _esSubscriptionName, 
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