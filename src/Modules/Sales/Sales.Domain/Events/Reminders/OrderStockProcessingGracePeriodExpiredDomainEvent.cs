using System;

using VShop.SharedKernel.Infrastructure.Messaging;
using VShop.SharedKernel.Infrastructure.Events.Contracts;

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