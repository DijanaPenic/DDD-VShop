using VShop.SharedKernel.Infrastructure.Events.Contracts;

namespace VShop.Modules.Billing.Integration.Events
{
    // Notification for Sales - need to update order status and then validate stock.
    public partial class PaymentFailedIntegrationEvent : IIntegrationEvent
    {
        public PaymentFailedIntegrationEvent(Guid orderId)
        {
            OrderId = orderId;
        }
    }
}