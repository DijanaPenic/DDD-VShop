using System;

using VShop.SharedKernel.Messaging.Events;

namespace VShop.Modules.Billing.Integration.Events
{
    // Notification for Sales - need to cancel the order eventually.
    public record PaymentSucceededIntegrationEvent : IntegrationEvent
    {
        public Guid OrderId { get; }
        
        public PaymentSucceededIntegrationEvent(Guid orderId)
        {
            OrderId = orderId;
        }
        
        public PaymentSucceededIntegrationEvent(Guid orderId, Guid causationId, Guid correlationId)
        {
            OrderId = orderId;
            causationId = causationId;
            CorrelationId = correlationId;
        }
    }
}