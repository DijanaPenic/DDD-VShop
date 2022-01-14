using MediatR;

namespace VShop.SharedKernel.Messaging.Events.Publishing.Contracts
{
    public interface IEventHandler<in TEvent> : INotificationHandler<TEvent> where TEvent : IBaseEvent
    {
        
    }
}