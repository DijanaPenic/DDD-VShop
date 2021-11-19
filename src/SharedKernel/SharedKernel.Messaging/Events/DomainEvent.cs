using MediatR;

namespace VShop.SharedKernel.Messaging.Events
{
    public abstract record DomainEvent : Message, IDomainEvent
    {
        
    }
    
    public interface IDomainEvent : IBaseEvent, INotification
    {
    
    }
}