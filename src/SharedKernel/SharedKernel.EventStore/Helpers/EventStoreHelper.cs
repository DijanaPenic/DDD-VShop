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
        public static EventData[] PrepareMessageData<TMessage>(params TMessage[] messages)
            where TMessage : class, IMessage
        {
            if (messages == null || !messages.Any()) return Array.Empty<EventData>();

            return messages.Select((@event, index) =>
                {
                    string eventName = MessageTypeMapper.ToName(@event.GetType());
                    Guid eventId = DeterministicGuid.Create(@event.CausationId, $"{eventName}-{index}"); // TODO - can this be improved?

                    return new EventData
                    (
                        eventId,
                        eventName,
                        true,
                        Serialize(@event),
                        Serialize(GetMetadata(@event))
                    );
                }).ToArray();
        }

        private static MessageMetadata GetMetadata(IMessage message)
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