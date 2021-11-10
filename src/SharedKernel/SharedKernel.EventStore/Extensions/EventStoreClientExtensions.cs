using Polly;
using Polly.Retry;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using EventStore.Client;

using VShop.SharedKernel.EventStore.Helpers;
using VShop.SharedKernel.Infrastructure.Messaging;

namespace VShop.SharedKernel.EventStore.Extensions
{
    public static class EventStoreClientExtensions
    {
        private const int MaxRetryAttempts = 3;
        private static readonly TimeSpan PauseBetweenFailures = TimeSpan.FromSeconds(2);
        
        private static readonly AsyncRetryPolicy RetryPolicy = Policy
            .Handle<Exception>(ex => ex is not WrongExpectedVersionException)
            .WaitAndRetryAsync(MaxRetryAttempts, _ => PauseBetweenFailures);
        
        public static async Task AppendToStreamWithRetryAsync<TMessage>
        (
            this EventStoreClient eventStoreClient,
            string streamName,
            int expectedRevision,
            CancellationToken cancellationToken = default,
            params TMessage[] messages
        ) where TMessage : IMessage
        {
            EventData[] streamMessages = EventStoreHelper.PrepareMessageData(messages);

            await RetryPolicy.ExecuteAsync(async () =>
            {
                await eventStoreClient.AppendToStreamAsync
                (
                    streamName,
                    (ulong)expectedRevision,
                    streamMessages,
                    cancellationToken: cancellationToken
                );
            });
        }
        
        public static async Task AppendToStreamWithRetryAsync<TMessage>
        (
            this EventStoreClient eventStoreClient,
            string streamName,
            StreamState expectedState,
            CancellationToken cancellationToken = default,
            params TMessage[] messages
        ) where TMessage : IMessage
        {
            EventData[] streamMessages = EventStoreHelper.PrepareMessageData(messages);

            await RetryPolicy.ExecuteAsync(async () =>
            {
                await eventStoreClient.AppendToStreamAsync
                (
                    streamName,
                    expectedState,
                    streamMessages,
                    cancellationToken: cancellationToken
                );
            });
        }

        public static async Task<IEnumerable<TMessage>> ReadStreamForwardAsync<TMessage>
        (
            this EventStoreClient eventStoreClient,
            string streamName,
            StreamPosition position,
            CancellationToken cancellationToken = default
        ) where TMessage : class, IMessage
        {
            IList<ResolvedEvent> events = await eventStoreClient.ReadStreamAsync
            (
                Direction.Forwards,
                streamName,
                position,
                cancellationToken: cancellationToken
            ).ToListAsync(cancellationToken);
            
            return events.Select(@event => @event.DeserializeMessage() as TMessage); // TODO - refactor
        }
    }
}