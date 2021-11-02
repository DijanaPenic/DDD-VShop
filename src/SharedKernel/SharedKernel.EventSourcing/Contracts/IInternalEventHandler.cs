using VShop.SharedKernel.Infrastructure.Messaging;

namespace VShop.SharedKernel.EventSourcing.Contracts
{
    public interface IInternalEventHandler
    {
        void Handle(IDomainEvent @event); 
    }
}