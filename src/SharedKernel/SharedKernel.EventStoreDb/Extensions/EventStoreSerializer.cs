using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Newtonsoft.Json;
using EventStore.Client;

using VShop.SharedKernel.Messaging;
using VShop.SharedKernel.EventStoreDb.Messaging;
using VShop.SharedKernel.Infrastructure.Helpers;
using VShop.SharedKernel.Infrastructure.Serialization;

namespace VShop.SharedKernel.EventStoreDb.Extensions
{
    public static class EventStoreSerializer
    {
        private static readonly JsonSerializerSettings SerializerSettings = GetJsonSerializerSettings();
        
        public static T DeserializeData<T>(this ResolvedEvent resolvedEvent) 
            => (T)DeserializeData(resolvedEvent);
        
        public static object DeserializeData(this ResolvedEvent resolvedEvent)
        {
            Type eventType = MessageTypeMapper.ToType(resolvedEvent.Event.EventType);
            string jsonData = Encoding.UTF8.GetString(resolvedEvent.Event.Data.Span);
            object data = JsonConvert.DeserializeObject(jsonData, eventType);

            if (data is not IMessage message) return data;
            
            IMessageMetadata metadata = resolvedEvent.DeserializeMetadata();
                
            message.CausationId = metadata.CausationId;
            message.CorrelationId = metadata.CorrelationId;

            return message;
        }
        
        public static IMessageMetadata DeserializeMetadata(this ResolvedEvent resolvedEvent)
        {
            string jsonData = Encoding.UTF8.GetString(resolvedEvent.Event.Metadata.Span);
            
            return JsonConvert.DeserializeObject<MessageMetadata>(jsonData);
        }
        
        public static EventData ToEventData(this object message) => new
        (
            Uuid.FromGuid(SequentialGuid.Create()),
            MessageTypeMapper.ToName(message.GetType()),
            Serialize(message, SerializerSettings),
            Serialize(new { })
        );

        public static IEnumerable<EventData> ToEventData<TMessage>(this IEnumerable<TMessage> messages)
            where TMessage : IMessage
            => messages.Select(message => new EventData
            (
                Uuid.FromGuid(message.MessageId),
                MessageTypeMapper.ToName(message.GetType()),
                Serialize(message, SerializerSettings),
                Serialize(GetMetadata(message))
            ));

        private static byte[] Serialize(object data, JsonSerializerSettings serializerSettings)
            => Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data, serializerSettings));
        
        private static byte[] Serialize(object data)
            => Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data));

        private static JsonSerializerSettings GetJsonSerializerSettings()
        {
            PropertyIgnoreContractResolver jsonResolver = new();
            jsonResolver.Ignore(typeof(Message));

            JsonSerializerSettings serializerSettings = new() { ContractResolver = jsonResolver };

            return serializerSettings;
        }
        
        private static IMessageMetadata GetMetadata(IMessage message)
            => new MessageMetadata
            {
                EffectiveTime = DateTime.UtcNow,
                MessageId = message.MessageId,
                CausationId = message.CausationId,
                CorrelationId = message.CorrelationId
            };
    }
}