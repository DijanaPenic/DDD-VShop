using Polly;
using Polly.Retry;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using EventStore.ClientAPI;
using EventStore.ClientAPI.Exceptions;

using VShop.SharedKernel.EventStore.Helpers;
using VShop.SharedKernel.Infrastructure.Messaging;

namespace VShop.SharedKernel.EventStore.Extensions
{
    public static class EventStoreConnectionExtensions
    {
        public static async Task<List<TMessage>> ReadStreamEventsForwardAsync<TMessage>
        (
            this IEventStoreConnection eventStoreConnection,
            string stream
        ) where TMessage : class, IMessage
        {
            const int maxSliceSize = 4096;

            long position = 0L;
            bool endOfStream;
            List<TMessage> events = new();

            do
            {
                StreamEventsSlice slice = await eventStoreConnection.ReadStreamEventsForwardAsync
                (
                    stream,
                    position,
                    maxSliceSize,
                    false
                );

                position = slice.NextEventNumber;
                endOfStream = slice.IsEndOfStream;

                events.AddRange(slice.Events.Select(@event => @event.DeserializeMessage() as TMessage));
            } while (!endOfStream);

            return events;
        }

        public static async Task AppendToStreamAsync<TMessage>
        (
            this IEventStoreConnection eventStoreConnection,
            string stream,
            long expectedVersion,
            params TMessage[] messages
        ) where TMessage : IMessage
        {
            EventData[] streamMessages = EventStoreHelper.PrepareMessageData(messages);
            
            const int maxRetryAttempts = 3;
            TimeSpan pauseBetweenFailures = TimeSpan.FromSeconds(2);

            AsyncRetryPolicy retryPolicy = Policy
                .Handle<Exception>(ex => ex is not WrongExpectedVersionException)
                .WaitAndRetryAsync(maxRetryAttempts, _ => pauseBetweenFailures);

            await retryPolicy.ExecuteAsync(async () =>
            {
                await eventStoreConnection.AppendToStreamAsync
                (
                    stream,
                    expectedVersion,
                    streamMessages
                );
            });
        }
    }
}