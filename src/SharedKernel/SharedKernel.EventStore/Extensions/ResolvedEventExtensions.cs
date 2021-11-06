using System;
using System.Text;
using Newtonsoft.Json;
using EventStore.ClientAPI;

using VShop.SharedKernel.EventStore.Helpers;
using VShop.SharedKernel.EventSourcing.Messaging;

namespace VShop.SharedKernel.EventStore.Extensions
{
    public static class ResolvedEventExtensions
    {
        public static object DeserializeData(this ResolvedEvent resolvedEvent)
        {
            Type dataType = MessageTypeMapper.ToType(resolvedEvent.Event.EventType);
            string jsonData = Encoding.UTF8.GetString(resolvedEvent.Event.Data);
            
            return JsonConvert.DeserializeObject(jsonData, dataType);
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