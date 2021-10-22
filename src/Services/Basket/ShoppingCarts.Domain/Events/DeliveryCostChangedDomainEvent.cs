using System;

using VShop.SharedKernel.Infrastructure.Domain;

namespace VShop.Services.ShoppingCarts.Domain.Events
{
    public record DeliveryCostChangedDomainEvent : IDomainEvent
    {
        public Guid BasketId { get; init; }
        public decimal DeliveryCost { get; init; }
    }
}