using VShop.SharedKernel.Infrastructure.Events.Contracts;

namespace VShop.Modules.Billing.Integration.Events
{
    // Notification for Sales - need to cancel the order eventually.
    public partial class PaymentSucceededIntegrationEvent : IIntegrationEvent
    {
        public PaymentSucceededIntegrationEvent(Guid orderId)
        {
            OrderId = orderId;
        }
    }
}