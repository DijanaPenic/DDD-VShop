using MediatR;

namespace VShop.SharedKernel.Infrastructure.Messaging.Events
{
    public interface IIntegrationEvent : IMessage, INotification
    {
        
    }
}