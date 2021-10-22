using System;

using VShop.SharedKernel.Infrastructure.Domain;

namespace VShop.Services.Basket.Domain.Events
{
    public record DeliveryCostChangedDomainEvent : IDomainEvent
    {
        public Guid BasketId { get; init; }
        public decimal DeliveryCost { get; init; }
    }
}