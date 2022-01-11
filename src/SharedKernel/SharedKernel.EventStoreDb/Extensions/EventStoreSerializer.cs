using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using NodaTime;
using NodaTime.Serialization.Protobuf;
using Google.Protobuf;
using EventStore.Client;

using VShop.SharedKernel.Messaging;
using VShop.SharedKernel.Infrastructure.Helpers;

using IProtoData = Google.Protobuf.IMessage;
using IMessage = VShop.SharedKernel.Messaging.IMessage;

namespace VShop.SharedKernel.EventStoreDb.Extensions
{
    public static class EventStoreSerializer
    {
        public static IIdentifiedMessage<TMessage> Deserialize<TMessage>(this ResolvedEvent resolvedEvent) 
            where TMessage : IMessage
        {
            object data = Deserialize
            (
                resolvedEvent.Event.Data.Span.ToArray(),
                MessageTypeMapper.ToType(resolvedEvent.Event.EventType)
            );

            if (data is not TMessage message) throw new Exception("Invalid message type!");

            MessageMetadata metadata = Deserialize<MessageMetadata>(resolvedEvent.Event.Metadata.Span.ToArray());

            return new IdentifiedMessage<TMessage>(message, metadata);
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
                    Serialize(message.Data as IProtoData), // TODO - casting??
                    Serialize(message.Metadata),
                    "application/octet-stream"
                );
            }).ToList();

        private static byte[] Serialize(IProtoData data) => data.ToByteArray();

        public static TData Deserialize<TData>(byte[] data) where TData : IProtoData
            => (TData)Deserialize(data, typeof(TData));
        
        public static IProtoData Deserialize(byte[] data, Type type)
        {
            IProtoData message = (IProtoData)Activator.CreateInstance(type);
            
            using MemoryStream stream = new(data);
            message.MergeFrom(stream);
            
            return message;
        }

        // TODO - test idempotent behaviour without determ. guid.
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