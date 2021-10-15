using System;
using System.Collections.Generic;

namespace VShop.Services.Basket.Infrastructure.Entities
{
    public class BasketDetails
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public Domain.Models.BasketAggregate.Basket.BasketStatus Status { get; set; }
        public ICollection<BasketDetailsProductItem> ProductItems { get; set; }
        public int Version { get; set; }
    }
}