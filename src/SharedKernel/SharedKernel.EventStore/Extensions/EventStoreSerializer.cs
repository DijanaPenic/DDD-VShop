using System;
using System.Text;
using Newtonsoft.Json;
using EventStore.Client;

using VShop.SharedKernel.EventSourcing.Messaging;
using VShop.SharedKernel.Infrastructure.Messaging;

namespace VShop.SharedKernel.EventStore.Extensions
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
        
        public static EventData ToEventData(this object message) => new
        (
            Uuid.NewUuid(),
            MessageTypeMapper.ToName(message.GetType()),
            Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message)),
            Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new { }))
        );
    }
}