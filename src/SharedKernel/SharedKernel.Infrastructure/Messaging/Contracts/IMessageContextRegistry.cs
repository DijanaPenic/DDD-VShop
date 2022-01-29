namespace VShop.SharedKernel.Infrastructure.Messaging.Contracts;

public interface IMessageContextRegistry
{
    void Set<TMessage>(TMessage message, IMessageContext context) where TMessage : IMessage;
}