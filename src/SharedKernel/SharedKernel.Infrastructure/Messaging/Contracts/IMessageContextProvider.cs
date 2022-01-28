namespace VShop.SharedKernel.Infrastructure.Messaging.Contracts;

public interface IMessageContextProvider
{
    IMessageContext Get(IMessage message);
}