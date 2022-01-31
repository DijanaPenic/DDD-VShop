using VShop.SharedKernel.Infrastructure.Events.Contracts;

namespace VShop.Modules.ProcessManager.Infrastructure.Messages.Reminders
{
    internal partial class ShippingGracePeriodExpiredDomainEvent : IDomainEvent
    {
        public ShippingGracePeriodExpiredDomainEvent(Guid orderId)
        {
            OrderId = orderId;
        }
    }
}