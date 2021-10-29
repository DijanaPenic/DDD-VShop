﻿using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using EventStore.ClientAPI;

using VShop.SharedKernel.Infrastructure;

namespace VShop.SharedKernel.EventStore.Extensions
{
    public static class EventStoreConnectionExtensions
    {
        public static async Task<List<TMessage>> ReadStreamEventsForwardAsync<TMessage>
        (
            this IEventStoreConnection esConnection,
            string streamName
        ) where TMessage : class, IMessage
        {
            const int maxSliceSize = 4096;

            long position = 0L;
            bool endOfStream;
            List<TMessage> events = new();

            do
            {
                StreamEventsSlice slice = await esConnection.ReadStreamEventsForwardAsync
                (
                    streamName,
                    position,
                    maxSliceSize,
                    false
                );

                position = slice.NextEventNumber;
                endOfStream = slice.IsEndOfStream;

                events.AddRange(slice.Events.Select(@event => @event.DeserializeData() as TMessage));
            } while (!endOfStream);

            return events;
        }
    }
}