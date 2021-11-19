using MediatR;

namespace VShop.SharedKernel.Messaging.Events
{
    public abstract record IntegrationEvent : Message, IIntegrationEvent
    {
        
    }
    
    public interface IIntegrationEvent : IBaseEvent, INotification
    {
        
    }
}