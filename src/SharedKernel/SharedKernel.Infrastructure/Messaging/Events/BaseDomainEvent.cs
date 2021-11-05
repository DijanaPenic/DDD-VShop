namespace VShop.SharedKernel.Infrastructure.Messaging.Events
{
    public abstract record BaseDomainEvent : BaseMessage, IDomainEvent
    {
        
    }
}