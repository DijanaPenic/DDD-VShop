using System;
using System.Text;
using Newtonsoft.Json;
using EventStore.Client;

using VShop.SharedKernel.EventSourcing.Messaging;
using VShop.SharedKernel.Infrastructure.Messaging;

namespace VShop.SharedKernel.EventStore.Extensions
{
    public static class EventExtensions
    {
        // TODO - IMPORTANT - this needs to be refactored.
        public static T Deserialize<T>(this ResolvedEvent resolvedEvent) => (T)Deserialize(resolvedEvent); // TODO - will need to add checkpoint to the list
        
        public static object Deserialize(this ResolvedEvent resolvedEvent)
        {
            Type eventType = MessageTypeMapper.ToType(resolvedEvent.Event.EventType);
            string jsonData = Encoding.UTF8.GetString(resolvedEvent.Event.Data.Span);

            return JsonConvert.DeserializeObject(jsonData, eventType);
        }
        
        public static EventData ToJsonEventData(this object @event) => new
        (
            Uuid.NewUuid(),
            MessageTypeMapper.ToName(@event.GetType()),
            Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(@event)),
            Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new { }))
        );
        
        public static IMessage DeserializeMessage(this ResolvedEvent resolvedEvent)
        {
            IMessage message = resolvedEvent.Deserialize<IMessage>();
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
    }
}