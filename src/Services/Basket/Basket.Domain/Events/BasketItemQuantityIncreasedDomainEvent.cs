using System;

using VShop.SharedKernel.Infrastructure.Domain;

namespace VShop.Services.Basket.Domain.Events
{
    public record BasketItemQuantityIncreasedDomainEvent : IDomainEvent
    {
        public Guid BasketId { get; init; }
        public Guid ProductId { get; init; }
        public int Quantity { get; init; }
    }
}