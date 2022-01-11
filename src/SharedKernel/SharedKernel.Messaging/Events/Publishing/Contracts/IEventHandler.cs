using MediatR;

namespace VShop.SharedKernel.Messaging.Events.Publishing.Contracts
{
    public interface IEventHandler<TEvent> : INotificationHandler<IdentifiedEvent<TEvent>> 
        where TEvent : IBaseEvent
    {
        
    }
}