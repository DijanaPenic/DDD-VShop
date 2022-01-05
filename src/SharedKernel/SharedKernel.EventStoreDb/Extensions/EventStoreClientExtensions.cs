using NodaTime;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using EventStore.Client;

using VShop.SharedKernel.Messaging;
using VShop.SharedKernel.EventStoreDb.Policies;

namespace VShop.SharedKernel.EventStoreDb.Extensions
{
    public static class EventStoreClientExtensions
    {
        public static Task AppendToStreamAsync<TMessage>
        (
            this EventStoreClient eventStoreClient,
            string streamName,
            int expectedRevision,
            IEnumerable<TMessage> messages,
            Instant now,
            CancellationToken cancellationToken = default
        ) 
            where TMessage : IMessage
            => RetryWrapper.ExecuteAsync((ct) => eventStoreClient.AppendToStreamAsync
            (
                $"{eventStoreClient.ConnectionName}/{streamName}",
                StreamRevision.FromInt64(expectedRevision),
                messages.ToEventData(now),
                cancellationToken: ct
            ), cancellationToken);
        
        public static Task AppendToStreamAsync<TMessage>
        (
            this EventStoreClient eventStoreClient,
            string streamName,
            StreamState expectedState,
            IEnumerable<TMessage> messages,
            Instant now,
            CancellationToken cancellationToken = default
        ) 
            where TMessage : IMessage
            => RetryWrapper.ExecuteAsync((ct) => eventStoreClient.AppendToStreamAsync
            (
                $"{eventStoreClient.ConnectionName}/{streamName}",
                expectedState,
                messages.ToEventData(now),
                cancellationToken: ct
            ), cancellationToken); 
        
        public static async Task<IReadOnlyList<TMessage>> ReadStreamForwardAsync<TMessage>
        (
            this EventStoreClient eventStoreClient,
            string streamName,
            StreamPosition position,
            CancellationToken cancellationToken = default
        ) where TMessage : class, IMessage
        {
            EventStoreClient.ReadStreamResult result = await RetryWrapper.ExecuteAsync((ct) => eventStoreClient.ReadStreamAsync
            (
                Direction.Forwards,
                $"{eventStoreClient.ConnectionName}/{streamName}",
                position,
                cancellationToken: ct
            ), cancellationToken);

            if ((await result.ReadState) is ReadState.StreamNotFound) return new List<TMessage>();

            IList<ResolvedEvent> messages = await result.ToListAsync(cancellationToken);

            return messages.Select(@event => @event.DeserializeData<TMessage>()).ToList();
        }
    }
}