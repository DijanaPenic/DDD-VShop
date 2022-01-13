using System;
using System.Linq;
using System.Collections.Generic;
using EventStore.Client;

using VShop.SharedKernel.Messaging;
using VShop.SharedKernel.Infrastructure.Helpers;
using VShop.SharedKernel.Infrastructure.Serialization;

namespace VShop.SharedKernel.EventStoreDb.Extensions
{
    public static class EventStoreSerializer
    {
        public static IdentifiedMessage<TMessage> Deserialize<TMessage>(this ResolvedEvent resolvedEvent) 
            where TMessage : IMessage
        {
            object data = ProtobufSerializer.FromByteArray
            (
                resolvedEvent.Event.Data.Span.ToArray(),
                MessageTypeMapper.ToType(resolvedEvent.Event.EventType)
            );

            if (data is not TMessage message) throw new Exception("Invalid message type!");

            MessageMetadata metadata = ProtobufSerializer.FromByteArray
                <MessageMetadata>(resolvedEvent.Event.Metadata.Span.ToArray());

            return new IdentifiedMessage<TMessage>(message, metadata);
        }

        public static IReadOnlyList<EventData> ToEventData<TMessage>(this IEnumerable<IIdentifiedMessage<TMessage>> messages) 
            where TMessage : IMessage
            => messages.Select((message, index) => new EventData
                (
                    GetDeterministicMessageId(message, index),
                    GetMessageTypeName(message.Data),
                    ProtobufSerializer.ToByteArray(message.Data),
                    ProtobufSerializer.ToByteArray(message.Metadata),
                    "application/octet-stream"
                )).ToList();

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