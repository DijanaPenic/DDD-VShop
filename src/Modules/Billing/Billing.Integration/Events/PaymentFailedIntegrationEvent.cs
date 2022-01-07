using System;

using VShop.SharedKernel.Messaging.Events;

namespace VShop.Modules.Billing.Integration.Events
{
    // Notification for Sales - need to update order status and then validate stock.
    public record PaymentFailedIntegrationEvent : IntegrationEvent
    {
        public Guid OrderId { get; }
        
        public PaymentFailedIntegrationEvent(Guid orderId, Guid causationId = default, Guid correlationId = default)
        {
            OrderId = orderId;
            CausationId = causationId;
            CorrelationId = correlationId;
        }
    }
}