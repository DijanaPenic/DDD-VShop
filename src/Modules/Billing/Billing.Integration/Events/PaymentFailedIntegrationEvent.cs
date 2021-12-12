using System;

using VShop.SharedKernel.Messaging.Events;

namespace VShop.Modules.Billing.Integration.Events
{
    public record PaymentFailedIntegrationEvent : IntegrationEvent
    {
        public Guid OrderId { get; }

        public PaymentFailedIntegrationEvent(Guid orderId)
        {
            OrderId = orderId;
        }
    }
}