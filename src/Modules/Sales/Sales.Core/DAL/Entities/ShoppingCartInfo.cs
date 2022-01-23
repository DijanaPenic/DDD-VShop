using System;
using System.Collections.Generic;

using VShop.SharedKernel.PostgresDb;
using VShop.Modules.Sales.Domain.Enums;

namespace VShop.Modules.Sales.Core.DAL.Entities
{
    public class ShoppingCartInfo : DbEntityBase
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public ShoppingCartStatus Status { get; set; }
        public ICollection<ShoppingCartInfoItem> Items { get; set; }
    }
}