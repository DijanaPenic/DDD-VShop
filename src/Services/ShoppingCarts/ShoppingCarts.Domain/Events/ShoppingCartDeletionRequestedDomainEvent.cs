using System;

using VShop.SharedKernel.Domain;

namespace VShop.Services.ShoppingCarts.Domain.Events
{
    public record ShoppingCartDeletionRequestedDomainEvent : IDomainEvent
    {
        public Guid ShoppingCartId { get; init; }
    }
}