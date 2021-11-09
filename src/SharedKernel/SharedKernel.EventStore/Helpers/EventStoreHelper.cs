using System;
using System.Text;
using System.Linq;
using Newtonsoft.Json;
using EventStore.ClientAPI;

using VShop.SharedKernel.EventSourcing.Messaging;
using VShop.SharedKernel.Infrastructure.Messaging;
using VShop.SharedKernel.Infrastructure.Serialization;

namespace VShop.SharedKernel.EventStore.Helpers
{
    // TODO - potentially rename this class and directory
    public static class EventStoreHelper
    {
        public static EventData[] PrepareMessageData<TMessage>(params TMessage[] messages)
            where TMessage : IMessage
        {
            if (messages == null || !messages.Any()) return Array.Empty<EventData>();

            JsonSerializerSettings serializerSettings = GetJsonSerializerSettings();

            return messages.Select(message => new EventData
            (
                message.MessageId,
                MessageTypeMapper.ToName(message.GetType()),
                true,
                Serialize(message, serializerSettings),
                Serialize(GetMetadata(message), serializerSettings)
            )).ToArray();
        }

        private static IMessageMetadata GetMetadata(IMessage message)
         => new MessageMetadata
            {
                EffectiveTime = DateTime.UtcNow,
                MessageId = message.MessageId,
                CausationId = message.CausationId,
                CorrelationId = message.CorrelationId
            };

        private static byte[] Serialize(object data, JsonSerializerSettings serializerSettings)
            => Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data, serializerSettings));

        private static JsonSerializerSettings GetJsonSerializerSettings()
        {
            PropertyIgnoreContractResolver jsonResolver = new();
            jsonResolver.Ignore(typeof(BaseMessage));

            JsonSerializerSettings serializerSettings = new() { ContractResolver = jsonResolver };

            return serializerSettings;
        }
    }
}