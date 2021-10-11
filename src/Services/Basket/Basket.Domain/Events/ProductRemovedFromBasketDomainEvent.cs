using System;

using VShop.SharedKernel.Infrastructure.Domain;

namespace VShop.Services.Basket.Domain.Events
{
    public class ProductRemovedFromBasketDomainEvent : IDomainEvent
    {
        public Guid ProductId { get; set; }
    }
}