using System;
using System.Collections.Generic;

using VShop.SharedKernel.PostgresDb;

using static VShop.Services.ShoppingCarts.Domain.Models.BasketAggregate.ShoppingCart;

namespace VShop.Services.Basket.Infrastructure.Entities
{
    public class BasketDetails : DbBaseEntity
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public BasketStatus Status { get; set; }
        public ICollection<BasketDetailsProductItem> ProductItems { get; set; }
    }
}