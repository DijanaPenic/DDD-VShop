using System;

using VShop.SharedKernel.Messaging;
using VShop.SharedKernel.Messaging.Events;

namespace VShop.Modules.Sales.Domain.Events
{
    public partial class OrderLineOutOfStockRemovedDomainEvent : MessageContext, IDomainEvent
    {
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