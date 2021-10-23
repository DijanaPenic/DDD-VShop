using System;

using VShop.SharedKernel.Infrastructure.Domain;

namespace VShop.Services.ShoppingCarts.Domain.Events
{
    public record ShoppingCartDeletionRequestedDomainEvent : IDomainEvent
    {
        public Guid ShoppingCartId { get; init; }
    }
}