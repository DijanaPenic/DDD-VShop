using System;

using VShop.SharedKernel.Domain;

namespace VShop.Services.Sales.Domain.Events
{
    public record ShoppingCartItemQuantityDecreasedDomainEvent : IDomainEvent
    {
        public Guid ShoppingCartId { get; init; }
        public Guid ProductId { get; init; }
        public int Quantity { get; init; }
    }
}