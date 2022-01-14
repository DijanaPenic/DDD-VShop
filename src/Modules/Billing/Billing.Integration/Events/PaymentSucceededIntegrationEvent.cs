using System;

using VShop.SharedKernel.Messaging;
using VShop.SharedKernel.Messaging.Events;

namespace VShop.Modules.Billing.Integration.Events
{
    // Notification for Sales - need to cancel the order eventually.
    public partial class PaymentSucceededIntegrationEvent : MessageContext, IIntegrationEvent
    {
        public PaymentSucceededIntegrationEvent(Guid orderId, MessageMetadata metadata)
        {
            OrderId = orderId;
            Metadata = metadata;
        }
    }
}