using System;
using VShop.SharedKernel.Infrastructure;

namespace VShop.Services.Basket.Domain.Events
{
    public record BasketCreatedDomainEvent : IDomainEvent
    {
        public Guid CustomerId { get; set; }
        public BasketItem[] Items { get; set; }
        
        public class BasketItem
        {
            public Guid ProductId { get; set; }
            public string Quantity { get; set; }
            public double Price { get; set; }
        }
    }
}