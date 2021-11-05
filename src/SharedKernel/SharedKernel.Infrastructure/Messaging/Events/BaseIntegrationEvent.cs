namespace VShop.SharedKernel.Infrastructure.Messaging.Events
{
    public abstract record BaseIntegrationEvent : BaseMessage, IIntegrationEvent
    {
        
    }
}