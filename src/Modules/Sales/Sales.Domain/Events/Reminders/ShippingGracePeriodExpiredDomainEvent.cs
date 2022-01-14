using System;

using VShop.SharedKernel.Messaging;
using VShop.SharedKernel.Messaging.Events;

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