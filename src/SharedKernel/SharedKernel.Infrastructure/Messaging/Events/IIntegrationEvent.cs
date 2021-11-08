using MediatR;

namespace VShop.SharedKernel.Infrastructure.Messaging.Events
{
    public interface IIntegrationEvent : IEvent, INotification
    {
        
    }
}