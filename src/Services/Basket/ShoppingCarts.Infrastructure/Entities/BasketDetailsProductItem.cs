using System;

using VShop.SharedKernel.PostgresDb;

namespace VShop.Services.Basket.Infrastructure.Entities
{
    public class BasketDetailsProductItem : DbBaseEntity
    {
        public Guid Id { get; set; }
        public Guid BasketDetailsId { get; set; }
        public BasketDetails BasketDetails { get; set; }
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}