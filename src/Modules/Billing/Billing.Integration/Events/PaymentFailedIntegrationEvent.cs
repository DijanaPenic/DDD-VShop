using VShop.SharedKernel.Infrastructure.Events;
using VShop.SharedKernel.Infrastructure.Messaging;

namespace VShop.Modules.Billing.Integration.Events
{
    // Notification for Sales - need to update order status and then validate stock.
    public partial class PaymentFailedIntegrationEvent : MessageContext, IIntegrationEvent
    {
        public PaymentFailedIntegrationEvent(Guid orderId, MessageMetadata metadata)
        {
            OrderId = orderId;
            Metadata = metadata;
        }
    }
}