using VShop.SharedKernel.EventStoreDb.Messaging;
using VShop.SharedKernel.Infrastructure.Contexts;
using VShop.SharedKernel.Infrastructure.Messaging;
using VShop.SharedKernel.Infrastructure.Messaging.Contracts;

namespace VShop.SharedKernel.EventStoreDb.Extensions
{
    internal static class MessageMetadataExtensions
    {
        public static IMessageContext ToMessageContext(this MessageMetadata messageMetadata)
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