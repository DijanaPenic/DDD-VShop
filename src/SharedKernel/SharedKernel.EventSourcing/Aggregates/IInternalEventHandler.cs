using VShop.SharedKernel.Messaging.Events;

namespace VShop.SharedKernel.EventSourcing.Aggregates
{
    public interface IInternalEventHandler
    {
        void Handle(IDomainEvent @event); 
    }
}