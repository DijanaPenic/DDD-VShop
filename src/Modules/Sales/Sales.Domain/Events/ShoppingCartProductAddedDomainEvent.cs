using System;

using VShop.SharedKernel.Messaging.Events;

namespace VShop.Modules.Sales.Domain.Events
{
    public partial class ShoppingCartProductAddedDomainEvent : IDomainEvent
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