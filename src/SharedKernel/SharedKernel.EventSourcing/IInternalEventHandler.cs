using VShop.SharedKernel.Infrastructure.Messaging;

namespace VShop.SharedKernel.EventSourcing
{
    public interface IInternalEventHandler
    {
        void Handle(IDomainEvent @event); 
    }
}