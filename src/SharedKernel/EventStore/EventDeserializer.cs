using System;
using System.Text;
using Newtonsoft.Json;
using EventStore.ClientAPI;

using VShop.SharedKernel.EventSourcing;

namespace VShop.SharedKernel.EventStore
{
    public static class EventDeserializer
    {
        public static object Deserialize(this ResolvedEvent resolvedEvent)
        {
            Type dataType = TypeMapper.GetType(resolvedEvent.Event.EventType);
            string jsonData = Encoding.UTF8.GetString(resolvedEvent.Event.Data);
            
            return JsonConvert.DeserializeObject(jsonData, dataType);
        }

        public static T Deserialize<T>(this ResolvedEvent resolvedEvent)
        {
            string jsonData = Encoding.UTF8.GetString(resolvedEvent.Event.Data);
            
            return JsonConvert.DeserializeObject<T>(jsonData);
        }
    }
}