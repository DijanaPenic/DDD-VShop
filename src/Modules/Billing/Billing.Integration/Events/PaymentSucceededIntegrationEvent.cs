using System;

using VShop.SharedKernel.Messaging.Events;

namespace VShop.Modules.Billing.Integration.Events
{
    // Notification for Sales - need to cancel the order eventually.
    public record PaymentSucceededIntegrationEvent : IIntegrationEvent
    {
        public Guid OrderId { get; }

        public PaymentSucceededIntegrationEvent(Guid orderId)
        {
            OrderId = orderId;
        }
    }
}