using VShop.SharedKernel.Infrastructure.Events;
using VShop.SharedKernel.Infrastructure.Messaging;

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