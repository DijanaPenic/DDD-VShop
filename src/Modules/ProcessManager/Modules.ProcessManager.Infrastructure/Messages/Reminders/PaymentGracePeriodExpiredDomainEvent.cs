using VShop.SharedKernel.Infrastructure.Events.Contracts;

namespace VShop.Modules.ProcessManager.Infrastructure.Messages.Reminders
{
    internal partial class PaymentGracePeriodExpiredDomainEvent : IDomainEvent
    {
        public PaymentGracePeriodExpiredDomainEvent(Guid orderId)
        {
            OrderId = orderId;
        }
    }
}