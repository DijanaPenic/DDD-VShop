namespace VShop.SharedKernel.Infrastructure.Messaging.Events
{
    public abstract record IntegrationEvent : Message, IIntegrationEvent
    {
        
    }
}