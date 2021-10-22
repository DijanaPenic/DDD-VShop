using System;

using VShop.SharedKernel.Infrastructure.Domain;

namespace VShop.Services.ShoppingCarts.Domain.Events
{
    public record BasketCheckoutRequestedDomainEvent : IDomainEvent
    {
        public Guid BasketId { get; init; }
        public DateTime ConfirmedAt { get; init; }
    }
}