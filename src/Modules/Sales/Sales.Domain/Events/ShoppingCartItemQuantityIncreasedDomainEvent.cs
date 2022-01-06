using System;

using VShop.SharedKernel.Messaging.Events;

namespace VShop.Modules.Sales.Domain.Events
{
    public record ShoppingCartItemQuantityIncreasedDomainEvent : DomainEvent
    {
        public Guid ShoppingCartId { get; }
        public Guid ProductId { get; }
        public int Quantity { get; }

        public ShoppingCartItemQuantityIncreasedDomainEvent
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