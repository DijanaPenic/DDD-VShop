using System;

using VShop.SharedKernel.Infrastructure.Events;
using VShop.SharedKernel.Infrastructure.Extensions;
using VShop.SharedKernel.Infrastructure.Messaging;

namespace VShop.Modules.Sales.Domain.Events
{
    public partial class OrderLineAddedDomainEvent : MessageContext, IDomainEvent
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