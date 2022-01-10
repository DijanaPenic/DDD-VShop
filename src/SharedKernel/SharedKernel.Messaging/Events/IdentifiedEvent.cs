using MediatR;

namespace VShop.SharedKernel.Messaging.Events
{
    public class IdentifiedEvent<TEvent> : IdentifiedMessage<TEvent>, IIdentifiedEvent<TEvent>
        where TEvent : IBaseEvent
    {
        public IdentifiedEvent(TEvent @event, MessageMetadata metadata) : base(@event, metadata) { }
    }

    public interface IIdentifiedEvent<out TEvent> : IIdentifiedMessage<TEvent>, INotification
        where TEvent : IBaseEvent
    {
        
    }
}