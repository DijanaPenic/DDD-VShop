using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using ProtoBuf;
using NodaTime;
using NodaTime.Serialization.Protobuf;
using Newtonsoft.Json;
using EventStore.Client;

using VShop.SharedKernel.Messaging;
using VShop.SharedKernel.Infrastructure.Helpers;
using VShop.SharedKernel.Messaging.Events;

namespace VShop.SharedKernel.EventStoreDb.Extensions
{
    public static class EventStoreSerializer
    {
        // TODO - need to review this.
        public static T DeserializeData<T>(this ResolvedEvent resolvedEvent) => (T)DeserializeData(resolvedEvent);
        
        public static object DeserializeData(this ResolvedEvent resolvedEvent)
        {
            Type eventType = MessageTypeMapper.ToType(resolvedEvent.Event.EventType);
            string jsonData = Encoding.UTF8.GetString(resolvedEvent.Event.Data.Span);
            object data = JsonConvert.DeserializeObject(jsonData, eventType);

            if (data is not IMessage message) return data;

            IIdentifiedMessage<IMessage> identifiedMessage = new IdentifiedMessage<IMessage>
            (
                message,
                resolvedEvent.DeserializeMetadata()
            );

            return identifiedMessage;
        }
        
        public static MessageMetadata DeserializeMetadata(this ResolvedEvent resolvedEvent)
        {
            string jsonData = Encoding.UTF8.GetString(resolvedEvent.Event.Metadata.Span);
            
            return JsonConvert.DeserializeObject<MessageMetadata>(jsonData);
        }

        public static IReadOnlyList<EventData> ToEventData<TMessage>
        (
            this IEnumerable<IIdentifiedMessage<TMessage>> messages,
            Instant now
        )
            where TMessage : IMessage
            => messages.Select((message, index) =>
            {
                message.Metadata.EffectiveTime = now.ToTimestamp(); // TODO - better way to handle this?

                return new EventData
                (
                    GetDeterministicMessageId(message, index),
                    GetMessageTypeName(message.Data),
                    Serialize(message.Data),
                    Serialize(message.Metadata),
                    "application/octet-stream"
                );
            }).ToList();

        private static byte[] Serialize(object data)
        {
            using MemoryStream stream = new();
            Serializer.Serialize(stream, data);
            
            return stream.ToArray();
        }

        private static Uuid GetDeterministicMessageId<TMessage>(IIdentifiedMessage<TMessage> message, int index) 
            where TMessage : IMessage
        {
            string messageName = GetMessageTypeName(message.Data);
            
            Guid deterministicId = DeterministicGuid.Create
            (
                message.Metadata.CausationId,
                $"{messageName}-{index}"
            );

            return Uuid.FromGuid(deterministicId);
        }

        private static string GetMessageTypeName(IMessage data) => MessageTypeMapper.ToName(data.GetType());
    }
}