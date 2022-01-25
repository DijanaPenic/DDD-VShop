using System;

using VShop.SharedKernel.Infrastructure.Messaging;
using VShop.SharedKernel.Infrastructure.Events.Contracts;

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