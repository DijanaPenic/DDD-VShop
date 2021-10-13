using System;

using VShop.SharedKernel.Infrastructure.Domain;

namespace VShop.Services.Basket.Domain.Events
{
    public class ProductRemovedFromBasketDomainEvent : IDomainEvent
    {
        public Guid BasketId { get; init; }
        public Guid ProductId { get; init; }
    }
}