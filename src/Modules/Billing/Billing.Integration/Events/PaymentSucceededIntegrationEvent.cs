using System;

using VShop.SharedKernel.Messaging.Events;

namespace VShop.Modules.Billing.Integration.Events
{
    public record PaymentSucceededIntegrationEvent : IntegrationEvent
    {
        public Guid OrderId { get; }
        
        public PaymentSucceededIntegrationEvent(Guid orderId)
        {
            OrderId = orderId;
        }
    }
}