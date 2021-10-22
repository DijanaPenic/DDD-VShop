using System;

using VShop.SharedKernel.Infrastructure.Domain;

namespace VShop.Services.ShoppingCarts.Domain.Events
{
    public record ProductAddedToBasketDomainEvent : IDomainEvent
    {
        public Guid BasketId { get; init; }
        public Guid BasketItemId { get; init; }
        public Guid ProductId { get; init; }
        public int Quantity { get; init; }
        public decimal UnitPrice { get; init; }
    }
}