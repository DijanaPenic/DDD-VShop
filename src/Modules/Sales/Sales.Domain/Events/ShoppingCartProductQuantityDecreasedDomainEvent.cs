using System;

using VShop.SharedKernel.Messaging.Events;

namespace VShop.Modules.Sales.Domain.Events
{
    public record ShoppingCartProductQuantityDecreasedDomainEvent : DomainEvent
    {
        public Guid ShoppingCartId { get; }
        public Guid ProductId { get; }
        public int Quantity { get; }

        public ShoppingCartProductQuantityDecreasedDomainEvent
        (
            Guid shoppingCartId,
            Guid productId,
            int quantity
        )
        {
            ShoppingCartId = shoppingCartId;
            ProductId = productId;
            Quantity = quantity;
        }
    }
}