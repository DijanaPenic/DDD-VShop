using MediatR;

namespace VShop.SharedKernel.Messaging.Events.Publishing.Contracts
{
    public interface IEventHandler<in TEvent> : INotificationHandler<IIdentifiedEvent<TEvent>>
        where TEvent : IBaseEvent
    {
        
    }
}