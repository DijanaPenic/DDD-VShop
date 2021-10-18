using System;
using System.Collections.Generic;

using static VShop.Services.Basket.Domain.Models.BasketAggregate.Basket;

namespace VShop.Services.Basket.Infrastructure.Entities
{
    public class BasketDetails
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public BasketStatus Status { get; set; }
        public ICollection<BasketDetailsProductItem> ProductItems { get; set; }
    }
}