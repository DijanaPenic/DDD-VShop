using System;
using System.Collections.Generic;

using VShop.SharedKernel.PostgresDb;
using VShop.Modules.Sales.Domain.Models.ShoppingCart;

namespace VShop.Modules.Sales.Infrastructure.Entities
{
    public class ShoppingCartInfo : DbBaseEntity
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public ShoppingCart.ShoppingCartStatus Status { get; set; }
        public ICollection<ShoppingCartInfoItem> Items { get; set; }
    }
}