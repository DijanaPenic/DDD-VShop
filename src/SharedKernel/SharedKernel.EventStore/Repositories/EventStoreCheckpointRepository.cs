using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using EventStore.ClientAPI;

using VShop.SharedKernel.EventStore.Extensions;
using VShop.SharedKernel.EventStore.Repositories.Contracts;
using VShop.SharedKernel.Infrastructure.Helpers;
using VShop.SharedKernel.Infrastructure.Extensions;

namespace VShop.SharedKernel.EventStore.Repositories
{
    public class EventStoreCheckpointRepository : IEventStoreCheckpointRepository
    {       
        private readonly IEventStoreConnection _esConnection;
        private readonly string _esCheckpointStreamName;

        public EventStoreCheckpointRepository
        (
            IEventStoreConnection esConnection,
            string esSubscriptionName
        )
        {
            _esConnection = esConnection;
            _esCheckpointStreamName = $"{esConnection.ConnectionName}/checkpoint/{esSubscriptionName}".ToSnakeCase();
        }

        public async Task<long?> GetCheckpointAsync()
        {
            StreamEventsSlice slice = await _esConnection.ReadStreamEventsBackwardAsync(_esCheckpointStreamName, -1, 1, false);
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
                GuidHelper.NewSequentialGuid(),
                "$checkpoint",
                true,
                Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(checkpointData)),
                null
            );

            return _esConnection.AppendToStreamAsync
            (
                _esCheckpointStreamName,
                ExpectedVersion.Any,
                @event
            );
        }

        private async Task SetStreamMaxCountAsync()
        {
            StreamMetadataResult metadata = await _esConnection.GetStreamMetadataAsync(_esCheckpointStreamName);

            if (!metadata.StreamMetadata.MaxCount.HasValue)
                await _esConnection.SetStreamMetadataAsync
                (
                    _esCheckpointStreamName, 
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