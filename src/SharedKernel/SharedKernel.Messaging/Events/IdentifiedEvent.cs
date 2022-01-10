using MediatR;

namespace VShop.SharedKernel.Messaging.Events
{
    public class IdentifiedEvent : IdentifiedMessage<IBaseEvent>, IIdentifiedEvent
    {
        public IdentifiedEvent(IBaseEvent @event, MessageMetadata metadata) : base(@event, metadata) { }
    }

    public interface IIdentifiedEvent : IIdentifiedMessage<IBaseEvent>, INotification
    {
        
    }
}