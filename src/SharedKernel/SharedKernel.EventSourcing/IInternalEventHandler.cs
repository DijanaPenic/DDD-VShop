using VShop.SharedKernel.Domain;

namespace VShop.SharedKernel.EventSourcing
{
    public interface IInternalEventHandler
    {
        void Handle(IDomainEvent @event); 
    }
}