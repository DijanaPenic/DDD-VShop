namespace VShop.SharedKernel.Infrastructure.Messaging
{
    public abstract record BaseDomainEvent : BaseMessage, IDomainEvent
    {
        
    }
}