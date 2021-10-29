using MediatR;

using VShop.SharedKernel.Infrastructure.Messaging;

namespace VShop.SharedKernel.Application.Events
{
    public interface IDomainEventHandler<in TEvent> : INotificationHandler<TEvent>
        where TEvent : IDomainEvent
    {
        
    }
}