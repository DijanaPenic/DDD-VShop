using System;

using VShop.SharedKernel.Messaging.Events;

namespace VShop.Modules.Sales.Domain.Events.Reminders
{
    public record OrderStockProcessingGracePeriodExpiredDomainEvent //: DomainEvent
    {
        public Guid OrderId { get; }

        public OrderStockProcessingGracePeriodExpiredDomainEvent(Guid orderId)
        {
            OrderId = orderId;
        }
    }
}