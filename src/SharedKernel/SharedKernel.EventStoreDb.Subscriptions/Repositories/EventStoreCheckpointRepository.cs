using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventStore.Client;

using VShop.SharedKernel.EventStoreDb.Extensions;
using VShop.SharedKernel.EventStoreDb.Subscriptions.Repositories.Contracts;
using VShop.SharedKernel.Infrastructure.Extensions;

namespace VShop.SharedKernel.EventStoreDb.Subscriptions.Repositories
{
    public record Checkpoint(string SubscriptionId, ulong? Position, DateTime DateUpdated);

    public class EventStoreCheckpointRepository : ICheckpointRepository
    {
        private readonly EventStoreClient _eventStoreClient;

        public EventStoreCheckpointRepository(EventStoreClient eventStoreClient)
            => _eventStoreClient = eventStoreClient;

        public async ValueTask<ulong?> LoadAsync(string subscriptionId, CancellationToken cancellationToken)
        {
            string streamName = GetStreamName(subscriptionId);

            EventStoreClient.ReadStreamResult result = _eventStoreClient.ReadStreamAsync
            (
                Direction.Backwards,
                streamName,
                StreamPosition.End,
                1,
                cancellationToken: cancellationToken
            );

            if ((await result.ReadState) is ReadState.StreamNotFound)
            {
                await SetStreamMaxCountAsync(streamName, cancellationToken);
                await SaveAsync(subscriptionId, StreamPosition.Start, cancellationToken);

                return null;
            }

            ResolvedEvent? @event = await result.FirstOrDefaultAsync(cancellationToken);

            return @event?.DeserializeData<Checkpoint>().Position;
        }

        public async ValueTask SaveAsync(string subscriptionId, ulong position, CancellationToken cancellationToken)
        {
            Checkpoint message = new(subscriptionId, position, DateTime.UtcNow);
            EventData[] messageToAppend = { message.ToEventData() };
            string streamName = GetStreamName(subscriptionId);

            // Store new checkpoint expecting stream to exist
            await _eventStoreClient.AppendToStreamAsync
            (
                streamName,
                StreamState.StreamExists,
                messageToAppend,
                cancellationToken: cancellationToken
            );
        }

        private Task SetStreamMaxCountAsync(string streamName, CancellationToken cancellationToken)
            => _eventStoreClient.SetStreamMetadataAsync
                (
                    streamName,
                    StreamState.NoStream,
                    new StreamMetadata(1),
                    cancellationToken: cancellationToken
                );

        private string GetStreamName(string subscriptionId) 
            => $"{_eventStoreClient.ConnectionName}/checkpoint/{subscriptionId}".ToSnakeCase();
    }
}