using MediatR;

namespace VShop.SharedKernel.Infrastructure.Messaging.Events.Publishing
{
    public interface IIntegrationEventHandler<in TEvent> : INotificationHandler<TEvent>
        where TEvent : IIntegrationEvent
    {
        
    }
}