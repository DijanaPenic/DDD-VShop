using System;

using VShop.SharedKernel.Messaging;
using VShop.SharedKernel.Messaging.Events;

namespace VShop.Modules.Sales.Domain.Events.Reminders
{
    public partial class PaymentGracePeriodExpiredDomainEvent : MessageContext, IDomainEvent
    {
        public PaymentGracePeriodExpiredDomainEvent(Guid orderId)
        {
            OrderId = orderId;
        }
    }
}