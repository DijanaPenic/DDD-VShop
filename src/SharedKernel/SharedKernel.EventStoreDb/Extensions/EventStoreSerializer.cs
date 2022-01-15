using System;
using System.Linq;
using System.Collections.Generic;
using EventStore.Client;

using VShop.SharedKernel.Messaging;

using VShop.SharedKernel.Infrastructure.Serialization;
using VShop.SharedKernel.Infrastructure.Types;

using Uuid = EventStore.Client.Uuid;
using IProtoData = Google.Protobuf.IMessage;

namespace VShop.SharedKernel.EventStoreDb.Extensions
{
    public static class EventStoreSerializer
    {
        public static TMessage Deserialize<TMessage>(this ResolvedEvent resolvedEvent) where TMessage : IMessage
        {
            object data = ProtobufSerializer.FromByteArray
            (
                resolvedEvent.Event.Data.Span.ToArray(),
                MessageTypeMapper.ToType(resolvedEvent.Event.EventType)
            );

            if (data is not TMessage message) return default;

            message.Metadata = ProtobufSerializer.FromByteArray
                <MessageMetadata>(resolvedEvent.Event.Metadata.Span.ToArray());

            return message;
        }

        public static IReadOnlyList<EventData> ToEventData<TMessage>(this IEnumerable<TMessage> messages) 
            where TMessage : IMessage
            => messages.Select((message, index) => new EventData
                (
                    GetDeterministicMessageId(message, index),
                    GetMessageTypeName(message),
                    ProtobufSerializer.ToByteArray(message),
                    ProtobufSerializer.ToByteArray(message.Metadata),
                    "application/octet-stream"
                )).ToList();

        // TODO - test idempotent behaviour without determ. guid.
        private static Uuid GetDeterministicMessageId<TMessage>(TMessage message, int index) where TMessage : IMessage
        {
            string messageName = GetMessageTypeName(message);
            
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