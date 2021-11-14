using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using EventStore.Client;

using VShop.SharedKernel.EventStore.Policies;
using VShop.SharedKernel.Infrastructure.Messaging;

namespace VShop.SharedKernel.EventStore.Extensions
{
    public static class EventStoreClientExtensions
    {
        public static async Task AppendToStreamAsync<TMessage>
        (
            this EventStoreClient eventStoreClient,
            string streamName,
            int expectedRevision,
            IEnumerable<TMessage> messages,
            CancellationToken cancellationToken = default
        ) where TMessage : IMessage
        {
            await RetryWrapper.ExecuteAsync((ct) => eventStoreClient.AppendToStreamAsync
            (
                streamName,
                StreamRevision.FromInt64(expectedRevision),
                messages.ToEventData(),
                cancellationToken: ct
            ), cancellationToken);
        }
        
        public static async Task AppendToStreamAsync<TMessage>
        (
            this EventStoreClient eventStoreClient,
            string streamName,
            StreamState expectedState,
            IEnumerable<TMessage> messages,
            CancellationToken cancellationToken = default
        ) where TMessage : IMessage
        {
            await RetryWrapper.ExecuteAsync((ct) => eventStoreClient.AppendToStreamAsync
            (
                streamName,
                expectedState,
                messages.ToEventData(),
                cancellationToken: ct
            ), cancellationToken);
        }

        public static async Task<IList<TMessage>> ReadStreamForwardAsync<TMessage>
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
                streamName,
                position,
                cancellationToken: ct
            ), cancellationToken);

            if ((await result.ReadState) is ReadState.StreamNotFound) return new List<TMessage>();

            IList<ResolvedEvent> messages = await result.ToListAsync(cancellationToken);

            return messages.Select(@event => @event.DeserializeData<TMessage>()).ToList();
        }
    }
}