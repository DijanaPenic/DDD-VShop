using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Newtonsoft.Json;
using EventStore.Client;

using VShop.SharedKernel.Messaging;
using VShop.SharedKernel.EventStoreDb.Messaging;
using VShop.SharedKernel.Infrastructure.Serialization;
using VShop.SharedKernel.Infrastructure.Services.Contracts;

namespace VShop.SharedKernel.EventStoreDb.Extensions
{
    public static class EventStoreSerializer
    {
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

        public static IEnumerable<EventData> ToEventData<TMessage>(this IEnumerable<TMessage> messages, IClockService clockService)
            where TMessage : IMessage
            => messages.Select(message => new EventData
            (
                Uuid.FromGuid(message.MessageId),
                MessageTypeMapper.ToName(message.GetType()),
                Serialize(message),
                Serialize(GetMetadata(clockService, message))
            ));

        private static byte[] Serialize(object data)
        {
            PropertyIgnoreContractResolver jsonResolver = new();
            jsonResolver.Ignore(typeof(Message));

            JsonSerializerSettings serializerSettings = (JsonConvert.DefaultSettings is null) 
                ? new JsonSerializerSettings() : JsonConvert.DefaultSettings();

            serializerSettings.ContractResolver = jsonResolver;

            return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data, serializerSettings));
        }

        private static IMessageMetadata GetMetadata(IClockService clockService, IMessage message)
            => new MessageMetadata
            {
                EffectiveTime = clockService.Now,
                MessageId = message.MessageId,
                CausationId = message.CausationId,
                CorrelationId = message.CorrelationId
            };
    }
}