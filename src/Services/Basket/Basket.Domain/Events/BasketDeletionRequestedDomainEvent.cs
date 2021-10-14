using System;

using VShop.SharedKernel.Infrastructure.Domain;

namespace VShop.Services.Basket.Domain.Events
{
    public record BasketDeletionRequestedDomainEvent : IDomainEvent
    {
        public Guid BasketId { get; init; }
    }
}