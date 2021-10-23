using System;
using System.Collections.Generic;

using VShop.SharedKernel.PostgresDb;

using static VShop.Services.ShoppingCarts.Domain.Models.ShoppingCartAggregate.ShoppingCart;

namespace VShop.Services.ShoppingCarts.Infrastructure.Entities
{
    public class ShoppingCart : DbBaseEntity
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public ShoppingCartStatus Status { get; set; }
        public ICollection<ShoppingCartItem> Items { get; set; }
    }
}