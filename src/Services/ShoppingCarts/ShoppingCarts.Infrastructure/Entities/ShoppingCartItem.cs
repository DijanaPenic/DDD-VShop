using System;

using VShop.SharedKernel.PostgresDb;

namespace VShop.Services.ShoppingCarts.Infrastructure.Entities
{
    public class ShoppingCartItem : DbBaseEntity
    {
        public Guid Id { get; set; }
        public Guid ShoppingCartId { get; set; }
        public ShoppingCart ShoppingCart { get; set; }
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}