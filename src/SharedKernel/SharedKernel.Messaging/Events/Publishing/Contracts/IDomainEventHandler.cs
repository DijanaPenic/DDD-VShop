using MediatR;

namespace VShop.SharedKernel.Messaging.Events.Publishing.Contracts
{
    public interface IDomainEventHandler<in TEvent> : INotificationHandler<TEvent>
        where TEvent : IDomainEvent
    {
        
    }
}