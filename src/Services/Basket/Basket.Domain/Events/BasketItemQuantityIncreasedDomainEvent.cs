using System;

using VShop.SharedKernel.Infrastructure.Domain;

namespace VShop.Services.Basket.Domain.Events
{
    public record BasketItemQuantityIncreasedDomainEvent : IDomainEvent
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
    }
}