using System;
using System.Text;
using System.Linq;
using Newtonsoft.Json;
using EventStore.ClientAPI;

using VShop.SharedKernel.EventSourcing;
using VShop.SharedKernel.Infrastructure.Helpers;
using VShop.SharedKernel.Infrastructure.Messaging;

namespace VShop.SharedKernel.EventStore.Helpers
{
    public static class EventStoreHelper
    {
        public static EventData[] PrepareEventData<TMessage>(params TMessage[] messages)
            where TMessage : class, IMessage
        {
            if (messages == null || !messages.Any()) return Array.Empty<EventData>();

            EventMetadata eventMetadata = new() { EffectiveTime = DateTime.UtcNow };

            EventData[] events = messages
                .Select(
                    @event =>
                        new EventData
                        (
                            GuidHelper.NewSequentialGuid(), // TODO - idempotency
                            EventTypeMapper.ToName(@event.GetType()),
                            true,
                            Serialize(@event),
                            Serialize(eventMetadata)
                        )
                )
                .ToArray();

            return events;
        }

        private static byte[] Serialize(object data) => Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data));
    }
}