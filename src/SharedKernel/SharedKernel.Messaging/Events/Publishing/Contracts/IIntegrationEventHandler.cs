using MediatR;

namespace VShop.SharedKernel.Messaging.Events.Publishing.Contracts
{
    public interface IIntegrationEventHandler<in TEvent> : INotificationHandler<TEvent>
        where TEvent : IIntegrationEvent
    {
        
    }
}