using System;

using VShop.SharedKernel.Infrastructure.Extensions;
using VShop.SharedKernel.Infrastructure.Messaging;
using VShop.SharedKernel.Infrastructure.Events.Contracts;

namespace VShop.Modules.Sales.Domain.Events
{
    internal partial class OrderLineAddedDomainEvent : IDomainEvent
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
            UnitPrice = unitPrice.ToMoney();
        }
    }
}