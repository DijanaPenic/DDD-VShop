using VShop.SharedKernel.Infrastructure.Events.Contracts;

namespace VShop.SharedKernel.EventSourcing.Aggregates
{
    public interface IInternalEventHandler
    {
        void Handle(IDomainEvent @event); 
    }
}