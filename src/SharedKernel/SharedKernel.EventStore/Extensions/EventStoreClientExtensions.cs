using Polly;
using Polly.Retry;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using EventStore.Client;

using VShop.SharedKernel.Infrastructure.Messaging;

namespace VShop.SharedKernel.EventStore.Extensions
{
    public static class EventStoreClientExtensions
    {
        private const int MaxRetryAttempts = 3;
        private static readonly TimeSpan PauseBetweenFailures = TimeSpan.FromSeconds(2);
        
        // TODO - this should be done for all EventStore calls, not just these ones.
        private static readonly AsyncRetryPolicy RetryPolicy = Policy
            .Handle<Exception>(ex => ex is not WrongExpectedVersionException)
            .WaitAndRetryAsync(MaxRetryAttempts, _ => PauseBetweenFailures);
        
        public static async Task AppendToStreamWithRetryAsync<TMessage>
        (
            this EventStoreClient eventStoreClient,
            string streamName,
            int expectedRevision,
            IEnumerable<TMessage> messages,
            CancellationToken cancellationToken = default
        ) where TMessage : IMessage
        {
            await RetryPolicy.ExecuteAsync(async (cToken) =>
            {
                await eventStoreClient.AppendToStreamAsync
                (
                    streamName,
                    StreamRevision.FromInt64(expectedRevision),
                    messages.ToEventData(),
                    cancellationToken: cToken
                );
            }, cancellationToken);
        }
        
        public static async Task AppendToStreamWithRetryAsync<TMessage>
        (
            this EventStoreClient eventStoreClient,
            string streamName,
            StreamState expectedState,
            IEnumerable<TMessage> messages,
            CancellationToken cancellationToken = default
        ) where TMessage : IMessage
        {
            await RetryPolicy.ExecuteAsync(async (cToken) =>
            {
                await eventStoreClient.AppendToStreamAsync
                (
                    streamName,
                    expectedState,
                    messages.ToEventData(),
                    cancellationToken: cToken
                );
            }, cancellationToken);
        }

        public static async Task<IList<TMessage>> ReadStreamForwardAsync<TMessage>
        (
            this EventStoreClient eventStoreClient,
            string streamName,
            StreamPosition position,
            CancellationToken cancellationToken = default
        ) where TMessage : class, IMessage
        {
            EventStoreClient.ReadStreamResult result = eventStoreClient.ReadStreamAsync
            (
                Direction.Forwards,
                streamName,
                position,
                cancellationToken: cancellationToken
            );

            if ((await result.ReadState) is ReadState.StreamNotFound) return new List<TMessage>();

            IList<ResolvedEvent> messages = await result.ToListAsync(cancellationToken);

            return messages.Select(@event => @event.DeserializeData<TMessage>()).ToList();
        }
    }
}