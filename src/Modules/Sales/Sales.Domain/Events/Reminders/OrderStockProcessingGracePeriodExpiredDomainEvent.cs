using System;

using VShop.SharedKernel.Infrastructure.Events;
using VShop.SharedKernel.Infrastructure.Messaging;

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