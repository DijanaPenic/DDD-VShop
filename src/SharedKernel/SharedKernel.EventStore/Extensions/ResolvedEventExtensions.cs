using System;
using System.Text;
using Newtonsoft.Json;
using EventStore.ClientAPI;

using VShop.SharedKernel.EventSourcing.Messaging;
using VShop.SharedKernel.Infrastructure.Messaging;

namespace VShop.SharedKernel.EventStore.Extensions
{
    public static class ResolvedEventExtensions
    {
        public static IMessage DeserializeMessage(this ResolvedEvent resolvedEvent)
        {
            Type dataType = MessageTypeMapper.ToType(resolvedEvent.Event.EventType);
            string jsonData = Encoding.UTF8.GetString(resolvedEvent.Event.Data);

            IMessage message = JsonConvert.DeserializeObject(jsonData, dataType) as IMessage;
            IMessageMetadata metadata = resolvedEvent.DeserializeMetadata();
            
            message.CausationId = metadata.CausationId;
            message.CorrelationId = metadata.CorrelationId;
            
            return message;
        }

        public static TMessage DeserializeData<TMessage>(this ResolvedEvent resolvedEvent)
        {
            string jsonData = Encoding.UTF8.GetString(resolvedEvent.Event.Data);
            
            return JsonConvert.DeserializeObject<TMessage>(jsonData);
        }
        
        public static IMessageMetadata DeserializeMetadata(this ResolvedEvent resolvedEvent)
        {
            string jsonData = Encoding.UTF8.GetString(resolvedEvent.Event.Metadata);
            
            return JsonConvert.DeserializeObject<MessageMetadata>(jsonData);
        }
    }
}