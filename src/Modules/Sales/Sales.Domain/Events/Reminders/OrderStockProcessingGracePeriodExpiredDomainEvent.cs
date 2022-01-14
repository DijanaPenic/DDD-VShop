using System;

using VShop.SharedKernel.Messaging;
using VShop.SharedKernel.Messaging.Events;

namespace VShop.Modules.Sales.Domain.Events.Reminders
{
    public partial class OrderStockProcessingGracePeriodExpiredDomainEvent : MessageContext, IDomainEvent
    {
        public OrderStockProcessingGracePeriodExpiredDomainEvent(Guid orderId)
        {
            OrderId = orderId;
        }
    }
}