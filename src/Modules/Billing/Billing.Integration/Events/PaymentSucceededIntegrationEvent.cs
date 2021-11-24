using System;

using VShop.SharedKernel.Messaging.Events;

namespace VShop.Modules.Billing.Integration.Events
{
    public record PaymentSucceededIntegrationEvent : IntegrationEvent
    {
        public Guid OrderId { get; }
        
        public PaymentSucceededIntegrationEvent(Guid orderId, Guid causationId, Guid correlationId)
        {
            OrderId = orderId;
            CausationId = causationId;
            CorrelationId = correlationId;
        }
    }
}