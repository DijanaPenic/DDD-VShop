using System;

using VShop.SharedKernel.Messaging;
using VShop.SharedKernel.Messaging.Events;
using VShop.SharedKernel.Infrastructure.Extensions;

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