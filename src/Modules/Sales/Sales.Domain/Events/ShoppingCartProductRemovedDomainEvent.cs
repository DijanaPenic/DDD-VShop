using System;

using VShop.SharedKernel.Messaging.Events;

namespace VShop.Modules.Sales.Domain.Events
{
    public record ShoppingCartProductRemovedDomainEvent //: DomainEvent
    {
        public Guid ShoppingCartId { get; }
        public Guid ProductId { get; }

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