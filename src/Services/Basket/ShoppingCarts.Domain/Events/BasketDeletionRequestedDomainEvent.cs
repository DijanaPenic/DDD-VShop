using System;

using VShop.SharedKernel.Infrastructure.Domain;

namespace VShop.Services.ShoppingCarts.Domain.Events
{
    public record BasketDeletionRequestedDomainEvent : IDomainEvent
    {
        public Guid BasketId { get; init; }
    }
}