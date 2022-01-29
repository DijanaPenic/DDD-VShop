using VShop.SharedKernel.Infrastructure.Messaging.Contracts;

namespace VShop.SharedKernel.Infrastructure.Messaging;

public record MessageEnvelope<TMessage>(TMessage Message, IMessageContext MessageContext)
    where TMessage : IMessage;