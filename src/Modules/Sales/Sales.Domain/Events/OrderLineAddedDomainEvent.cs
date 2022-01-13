using System;

using VShop.SharedKernel.Messaging.Events;

namespace VShop.Modules.Sales.Domain.Events
{
    public partial class OrderLineAddedDomainEvent : IDomainEvent
    {
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