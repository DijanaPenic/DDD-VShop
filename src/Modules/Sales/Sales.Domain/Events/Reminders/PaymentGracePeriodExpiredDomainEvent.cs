using System;

using VShop.SharedKernel.Infrastructure.Events;
using VShop.SharedKernel.Infrastructure.Messaging;

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