using System;
using System.Text;
using System.Linq;
using Newtonsoft.Json;
using EventStore.ClientAPI;

using VShop.SharedKernel.Infrastructure.Helpers;
using VShop.SharedKernel.Infrastructure.Messaging;
using VShop.SharedKernel.Infrastructure.Serialization;
using VShop.SharedKernel.EventSourcing.Messaging;

namespace VShop.SharedKernel.EventStore.Helpers
{
    // TODO - potentially rename this class and directory
    public static class EventStoreHelper
    {
        public static EventData[] PrepareMessageData<TMessage>(params TMessage[] messages)
            where TMessage : class, IMessage
        {
            if (messages == null || !messages.Any()) return Array.Empty<EventData>();
            
            PropertyIgnoreContractResolver jsonResolver = new();
            jsonResolver.Ignore(typeof(BaseMessage));

            JsonSerializerSettings serializerSettings = new() { ContractResolver = jsonResolver };

            return messages.Select((@event, index) =>
                {
                    string eventName = MessageTypeMapper.ToName(@event.GetType());
                    Guid eventId = DeterministicGuid.Create(@event.CausationId, $"{eventName}-{index}"); // TODO - can this be improved?

                    return new EventData
                    (
                        eventId,
                        eventName,
                        true,
                        Serialize(@event, serializerSettings),
                        Serialize(GetMetadata(@event), serializerSettings)
                    );
                }).ToArray();
        }

        private static IMessageMetadata GetMetadata(IMessage message)
         => new MessageMetadata()
            {
                EffectiveTime = DateTime.UtcNow,
                CausationId = message.CausationId,
                CorrelationId = message.CorrelationId
            };

        private static byte[] Serialize(object data, JsonSerializerSettings serializerSettings)
            => Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data, serializerSettings));
    }
}