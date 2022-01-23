using System;

using VShop.SharedKernel.Infrastructure.Events;
using VShop.SharedKernel.Infrastructure.Messaging;

namespace VShop.Modules.Sales.Domain.Events.Reminders
{
    public partial class ShippingGracePeriodExpiredDomainEvent : MessageContext, IDomainEvent
    {
        public ShippingGracePeriodExpiredDomainEvent(Guid orderId)
        {
            OrderId = orderId;
        }
    }
}