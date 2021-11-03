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

            EventData[] events = messages
                .Select(
                    @event =>
                        new EventData
                        (
                            SequentialGuid.Create(), // TODO - idempotency
                            MessageTypeMapper.ToName(@event.GetType()),
                            true,
                            Serialize(@event),
                            Serialize(GetMessageMetadata(@event))
                        )
                )
                .ToArray();

            return events;
        }

        private static MessageMetadata GetMessageMetadata(IMessage message)
         => new()
            {
                EffectiveTime = DateTime.UtcNow,
                CausationId = message.CausationId,
                CorrelationId = message.CorrelationId
            };

        private static byte[] Serialize(object data) 
            => Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data));
    }
}