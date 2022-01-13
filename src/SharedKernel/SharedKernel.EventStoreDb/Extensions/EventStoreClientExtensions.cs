using NodaTime;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using EventStore.Client;

using VShop.SharedKernel.Messaging;
using VShop.SharedKernel.EventStoreDb.Policies;
using VShop.SharedKernel.Infrastructure.Extensions;

namespace VShop.SharedKernel.EventStoreDb.Extensions
{
    public static class EventStoreClientExtensions
    {
        public static Task AppendToStreamAsync
        (
            this EventStoreClient eventStoreClient,
            string streamSuffix,
            int expectedRevision,
            IEnumerable<IIdentifiedMessage<IMessage>> messages,
            CancellationToken cancellationToken = default
        ) => RetryWrapper.ExecuteAsync((ct) => eventStoreClient.AppendToStreamAsync
            (
                eventStoreClient.GetStreamName(streamSuffix),
                StreamRevision.FromInt64(expectedRevision),
                messages.ToEventData(),
                cancellationToken: ct
            ), cancellationToken);
        
        public static Task AppendToStreamAsync
        (
            this EventStoreClient eventStoreClient,
            string streamSuffix,
            StreamState expectedState,
            IEnumerable<IIdentifiedMessage<IMessage>> messages,
            CancellationToken cancellationToken = default
        ) => RetryWrapper.ExecuteAsync((ct) => eventStoreClient.AppendToStreamAsync
            (
                eventStoreClient.GetStreamName(streamSuffix),
                expectedState,
                messages.ToEventData(),
                cancellationToken: ct
            ), cancellationToken);
        
        public static async Task<IReadOnlyList<IdentifiedMessage<TMessage>>> ReadStreamForwardAsync<TMessage>
        (
            this EventStoreClient eventStoreClient,
            string streamSuffix,
            CancellationToken cancellationToken = default
        ) where TMessage : IMessage
        {
            EventStoreClient.ReadStreamResult result = await RetryWrapper
                .ExecuteAsync((ct) => eventStoreClient.ReadStreamAsync
                (
                    Direction.Forwards,
                    eventStoreClient.GetStreamName(streamSuffix),
                    StreamPosition.Start,
                    cancellationToken: ct
                ), cancellationToken);

            if ((await result.ReadState) is ReadState.StreamNotFound) return new List<IdentifiedMessage<TMessage>>();

            IList<ResolvedEvent> messages = await result.ToListAsync(cancellationToken);

            return messages.Select(@event => @event.Deserialize<TMessage>()).ToList();
        }

        private static string GetStreamName(this EventStoreClientBase eventStoreClient, string value)
            => $"{eventStoreClient.ConnectionName}/{value}".ToSnakeCase();
    }
}