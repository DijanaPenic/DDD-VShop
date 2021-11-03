using MediatR;

namespace VShop.SharedKernel.Infrastructure.Messaging
{
    public interface IIntegrationEvent : IMessage, INotification
    {
        
    }
}