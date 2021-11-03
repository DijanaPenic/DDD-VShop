namespace VShop.SharedKernel.Infrastructure.Messaging
{
    public abstract record BaseIntegrationEvent : BaseMessage, IIntegrationEvent
    {
        
    }
}