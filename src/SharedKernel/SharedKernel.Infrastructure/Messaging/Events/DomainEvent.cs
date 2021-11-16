namespace VShop.SharedKernel.Infrastructure.Messaging.Events
{
    public abstract record DomainEvent : Message, IDomainEvent
    {
        
    }
}