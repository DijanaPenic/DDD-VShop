using System;

using VShop.SharedKernel.Messaging.Events;

namespace VShop.Modules.Sales.Domain.Events
{
    public record ShoppingCartProductAddedDomainEvent : DomainEvent
    {
        public Guid ShoppingCartId { get; }
        public Guid ProductId { get; }
        public int Quantity { get; }
        public decimal UnitPrice { get; }
        
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