using MediatR;

namespace VShop.SharedKernel.Infrastructure.Messaging.Events.Publishing
{
    public interface IDomainEventHandler<in TEvent> : INotificationHandler<TEvent>
        where TEvent : IDomainEvent
    {
        
    }
}