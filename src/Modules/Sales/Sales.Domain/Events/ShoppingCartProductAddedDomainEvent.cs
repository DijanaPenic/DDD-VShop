using System;

using VShop.SharedKernel.Infrastructure.Events;
using VShop.SharedKernel.Infrastructure.Extensions;
using VShop.SharedKernel.Infrastructure.Messaging;

namespace VShop.Modules.Sales.Domain.Events
{
    public partial class ShoppingCartProductAddedDomainEvent : MessageContext, IDomainEvent
    {
        public ShoppingCartProductAddedDomainEvent
        (
            Guid shoppingCartId,
            Guid productId,
            int quantity,
            decimal unitPrice
        )
        {
            ShoppingCartId = shoppingCartId;
            ProductId = productId;
            Quantity = quantity;
            UnitPrice = unitPrice.ToMoney();
        }
    }
}