using System.Collections.Generic;

namespace VShop.SharedKernel.Infrastructure.Messaging.Contracts;

public interface IMessageContextRegistry
{
    void Set<TMessage>(TMessage message, IMessageContext context) where TMessage : IMessage;
    void Set<TMessage>(IEnumerable<MessageEnvelope<TMessage>> messages) where TMessage : IMessage;
    IMessageContext Get(IMessage message);
}