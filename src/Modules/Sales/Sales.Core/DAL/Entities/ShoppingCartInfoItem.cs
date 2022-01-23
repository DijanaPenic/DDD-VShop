using System;

using VShop.SharedKernel.PostgresDb;

namespace VShop.Modules.Sales.Core.DAL.Entities
{
    public class ShoppingCartInfoItem : DbEntityBase
    {
        public Guid ProductId { get; set; }
        public Guid ShoppingCartInfoId { get; set; }
        public ShoppingCartInfo ShoppingCartInfo { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}