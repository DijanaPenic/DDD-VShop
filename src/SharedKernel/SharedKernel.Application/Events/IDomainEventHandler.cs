using MediatR;

using VShop.SharedKernel.Domain;

namespace VShop.SharedKernel.Application.Events
{
    public interface IDomainEventHandler<in TEvent> : INotificationHandler<TEvent>
        where TEvent : IDomainEvent
    {
        
    }
}