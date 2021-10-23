using System;

using VShop.SharedKernel.Domain;

namespace VShop.Services.ShoppingCarts.Domain.Events
{
    public record ShoppingCartCheckoutRequestedDomainEvent : IDomainEvent
    {
        public Guid ShoppingCartId { get; init; }
        public DateTime ConfirmedAt { get; init; }
    }
}