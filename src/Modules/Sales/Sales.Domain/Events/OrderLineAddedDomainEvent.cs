using System;

using VShop.SharedKernel.Messaging.Events;

namespace VShop.Modules.Sales.Domain.Events
{
    public record OrderLineAddedDomainEvent : DomainEvent
    {
        public Guid OrderId { get; }
        public Guid ProductId { get; }
        public int Quantity { get; }
        public decimal UnitPrice { get; }

        public OrderLineAddedDomainEvent
        (
            Guid orderId,
            Guid productId,
            int quantity,
            decimal unitPrice
        )
        {
            OrderId = orderId;
            ProductId = productId;
            Quantity = quantity;
            UnitPrice = unitPrice;
        }
    }
}