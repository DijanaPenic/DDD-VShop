using System;

using VShop.SharedKernel.Messaging.Events;

namespace VShop.Modules.Sales.Domain.Events
{
    public record OrderLineOutOfStockRemovedDomainEvent : DomainEvent
    {
        public Guid OrderId { get; }
        public Guid ProductId { get; }
        public int Quantity { get; }

        public OrderLineOutOfStockRemovedDomainEvent
        (
            Guid orderId,
            Guid productId,
            int quantity
        )
        {
            OrderId = orderId;
            ProductId = productId;
            Quantity = quantity;
        }
    }
}