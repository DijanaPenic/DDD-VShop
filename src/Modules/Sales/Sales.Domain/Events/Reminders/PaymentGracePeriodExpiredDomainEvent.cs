using System;

using VShop.SharedKernel.Messaging.Events;

namespace VShop.Modules.Sales.Domain.Events.Reminders
{
    public partial class PaymentGracePeriodExpiredDomainEvent : IDomainEvent
    {
        public PaymentGracePeriodExpiredDomainEvent(Guid orderId)
        {
            OrderId = orderId;
        }
    }
}