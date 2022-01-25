using System;
using System.Linq;
using System.Collections.Generic;
using EventStore.Client;
using Google.Protobuf;

using VShop.SharedKernel.Infrastructure.Types;
using VShop.SharedKernel.Infrastructure.Messaging;
using VShop.SharedKernel.Infrastructure.Messaging.Contracts;
using VShop.SharedKernel.Infrastructure.Serialization;

using Uuid = EventStore.Client.Uuid;
using IMessage = VShop.SharedKernel.Infrastructure.Messaging.Contracts.IMessage;

namespace VShop.SharedKernel.EventStoreDb.Extensions
{
    public static class EventStoreSerializer
    {
        public static TMessage Deserialize<TMessage>(this ResolvedEvent resolvedEvent, IMessageRegistry messageRegistry)
            where TMessage : IMessage
        {
            object data = ProtobufSerializer.FromByteArray
            (
                resolvedEvent.Event.Data.Span.ToArray(),
                messageRegistry.GetType(resolvedEvent.Event.EventType)
            );

            object UpcastMessage()
                => MessageTransformations.TryTransform
                (
                    resolvedEvent.Event.EventType,
                    data,
                    out object transformed
                ) ? transformed : data;

            if (UpcastMessage() is not TMessage message) return default;

            message.Metadata = ProtobufSerializer.FromByteArray
                <MessageMetadata>(resolvedEvent.Event.Metadata.Span.ToArray());

            return message;
        }

        public static IReadOnlyList<EventData> ToEventData<TMessage>
        (
            this IEnumerable<TMessage> messages,
            IMessageRegistry messageRegistry
        ) 
            where TMessage : IMessage
            => messages.Select((message, index) =>
            {
                string messageName = messageRegistry.GetName(message.GetType());

                return new EventData
                (
                    GetDeterministicMessageId(message, messageName, index),
                    messageName,
                    message.ToByteArray(), // Message.Metadata won't be deserialized here.
                    message.Metadata.ToByteArray(),
                    "application/octet-stream"
                );
            }).ToList();
        
        private static Uuid GetDeterministicMessageId<TMessage>(TMessage message, string messageName, int index) 
            where TMessage : IMessage
        {
            Guid deterministicId = DeterministicGuid.Create
            (
                message.Metadata.CausationId,
                $"{messageName}-{index}"
            );

            return Uuid.FromGuid(deterministicId);
        }
    }
}