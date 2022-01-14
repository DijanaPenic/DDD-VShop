using System;

using VShop.SharedKernel.Messaging;
using VShop.SharedKernel.Messaging.Events;

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
            UnitPrice = unitPrice;
        }
    }
}