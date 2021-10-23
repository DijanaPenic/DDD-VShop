using System;

using VShop.SharedKernel.Infrastructure.Domain;

namespace VShop.Services.ShoppingCarts.Domain.Events
{
    public record ShoppingCartItemQuantityDecreasedDomainEvent : IDomainEvent
    {
        public Guid ShoppingCartId { get; init; }
        public Guid ProductId { get; init; }
        public int Quantity { get; init; }
    }
}