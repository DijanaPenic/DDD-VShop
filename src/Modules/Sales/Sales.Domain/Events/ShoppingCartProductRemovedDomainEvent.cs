using System;

using VShop.SharedKernel.Messaging.Events;

namespace VShop.Modules.Sales.Domain.Events
{
    public partial class ShoppingCartProductRemovedDomainEvent : IDomainEvent
    {
        public ShoppingCartProductRemovedDomainEvent
        (
            Guid shoppingCartId,
            Guid productId
        )
        {
            ShoppingCartId = shoppingCartId;
            ProductId = productId;
        }
    }
}