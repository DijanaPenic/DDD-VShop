using VShop.SharedKernel.Infrastructure.Messaging;

namespace VShop.SharedKernel.EventSourcing.Aggregates
{
    public interface IInternalEventHandler
    {
        void Handle(IDomainEvent @event); 
    }
}