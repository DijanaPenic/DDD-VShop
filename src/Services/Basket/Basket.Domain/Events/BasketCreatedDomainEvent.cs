using System;

using VShop.SharedKernel.Infrastructure.Domain;

namespace VShop.Services.Basket.Domain.Events
{
    public record BasketCreatedDomainEvent : IDomainEvent
    {
        public Guid BasketId { get; set; }
        public Guid CustomerId { get; set; }
        public BasketItem[] BasketItems { get; set; }
        public int Discount { get; set; }
        
        public class BasketItem
        {
            public Guid ProductId { get; set; }
            public string Quantity { get; set; }
            public decimal UnitPrice { get; set; }
            public int Discount { get; set; }
        }
    }
}