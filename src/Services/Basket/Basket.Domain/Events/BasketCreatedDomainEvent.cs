using System;

using VShop.SharedKernel.Infrastructure.Domain;

namespace VShop.Services.Basket.Domain.Events
{
    public record BasketCreatedDomainEvent : IDomainEvent
    {
        public Guid BasketId { get; init; }
        public Guid CustomerId { get; init; }
        public BasketItem[] BasketItems { get; init; }
        public int CustomerDiscount { get; init; }
        
        // TODO - check if this needs to be removed
        public class BasketItem
        {
            public Guid ProductId { get; init; }
            public string Quantity { get; init; }
            public decimal UnitPrice { get; init; }
            public int Discount { get; init; }
        }
    }
}