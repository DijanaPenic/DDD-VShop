using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using NodaTime;
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
        public static T DeserializeData<T>(this ResolvedEvent resolvedEvent) => (T)DeserializeData(resolvedEvent);
        
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

        public static IReadOnlyList<EventData> ToEventData<TMessage>
        (
            this IEnumerable<TMessage> messages,
            Instant now
        )
            where TMessage : IMessage
            => messages.Select((message, index) => new EventData
            (
                GetDeterministicEventId(message, index),
                GetMessageName(message),
                Serialize(message),
                Serialize(GetMetadata(message, now))
            )).ToList();

        private static byte[] Serialize(object data)
        {
            PropertyIgnoreContractResolver jsonResolver = new();
            jsonResolver.Ignore(typeof(Message));

            JsonSerializerSettings serializerSettings = JsonConvert.DefaultSettings is null
                ? new JsonSerializerSettings() : JsonConvert.DefaultSettings();

            serializerSettings.ContractResolver = jsonResolver;

            return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data, serializerSettings));
        }

        private static Uuid GetDeterministicEventId(IMessage message, int index)
        {
            string messageName = GetMessageName(message);
            
            Guid deterministicId = DeterministicGuid.Create
            (
                message.CausationId,
                $"{messageName}-{index}"
            );

            return Uuid.FromGuid(deterministicId);
        }

        private static string GetMessageName(IMessage message) => MessageTypeMapper.ToName(message.GetType());

        private static IMessageMetadata GetMetadata(IMessage message, Instant now)
            => new MessageMetadata
            {
                EffectiveTime = now,
                MessageId = message.MessageId,
                CausationId = message.CausationId,
                CorrelationId = message.CorrelationId
            };
    }
}