using EventStore.Client;
using VShop.SharedKernel.EventStoreDb.Messaging;
using VShop.SharedKernel.Infrastructure.Contexts;
using VShop.SharedKernel.Infrastructure.Messaging;
using VShop.SharedKernel.Infrastructure.Messaging.Contracts;
using VShop.SharedKernel.Infrastructure.Serialization;

using IMessage = VShop.SharedKernel.Infrastructure.Messaging.Contracts.IMessage;

namespace VShop.SharedKernel.EventStoreDb.Extensions
{
    public static class EventStoreSerializer
    {
        public static MessageEnvelope<TMessage> Deserialize<TMessage>
        (
            this ResolvedEvent resolvedEvent,
            IMessageRegistry messageRegistry
        ) where TMessage : IMessage
        {
            object data = ProtobufSerializer.FromByteArray
            (
                resolvedEvent.Event.Data.Span.ToArray(),
                messageRegistry.GetType(resolvedEvent.Event.EventType)
            );

            object UpcastMessage() => messageRegistry.TryTransform
            (
                resolvedEvent.Event.EventType,
                data,
                out object transformed
            ) ? transformed : data;

            if (UpcastMessage() is not TMessage message) return default;

            MessageMetadata messageMetadata = ProtobufSerializer.FromByteArray
                <MessageMetadata>(resolvedEvent.Event.Metadata.Span.ToArray());

            return new MessageEnvelope<TMessage>(message, messageMetadata.ToMessageContext());
        }

        private static IMessageContext ToMessageContext(this MessageMetadata messageMetadata)
            => new MessageContext
            (
                messageMetadata.MessageId,
                new Context
                (
                    messageMetadata.CausationId,
                    messageMetadata.CorrelationId,
                    new IdentityContext(messageMetadata.UserId)
                )
            );
    }
}