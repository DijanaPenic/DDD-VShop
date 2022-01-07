using System;

using VShop.SharedKernel.Messaging.Events;

namespace VShop.Modules.Sales.Domain.Events
{
    public record ShoppingCartItemPriceChangedDomainEvent : DomainEvent
    {
        public Guid ShoppingCartId { get; }
        public Guid ProductId { get; }
        public decimal UnitPrice { get; }

        public ShoppingCartItemPriceChangedDomainEvent
        (
            Guid shoppingCartId,
            Guid productId,
            decimal unitPrice
        )
        {
            ShoppingCartId = shoppingCartId;
            ProductId = productId;
            UnitPrice = unitPrice;
        }
    }
}