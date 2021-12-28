using System;

using VShop.SharedKernel.Messaging.Events;

namespace VShop.Modules.Sales.Domain.Events
{
    public record StockConfirmationGracePeriodExpiredDomainEvent : DomainEvent
    {
        public Guid OrderId { get; }

        public StockConfirmationGracePeriodExpiredDomainEvent(Guid orderId)
        {
            OrderId = orderId;
        }
    }
}