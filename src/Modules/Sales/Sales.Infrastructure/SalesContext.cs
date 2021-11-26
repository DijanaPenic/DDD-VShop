using Microsoft.EntityFrameworkCore;

using VShop.SharedKernel.PostgresDb;
using VShop.Modules.Sales.Infrastructure.Entities;
using VShop.Modules.Sales.Infrastructure.EntityConfigurations;

namespace VShop.Modules.Sales.Infrastructure
{
    public class SalesContext : DbContextBase
    {
        public const string ShoppingCartSchema = "shopping_cart";
        public const string OrderSchema = "order";

        public DbSet<ShoppingCartInfo> ShoppingCarts { get; set; }
        public DbSet<ShoppingCartInfoItem> ShoppingCartItems { get; set; }

        public SalesContext(IDbContextBuilder contextBuilder) : base(contextBuilder) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => ContextBuilder.ConfigureContext(optionsBuilder);
    
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new ShoppingCartInfoEntityTypeConfiguration());
            builder.ApplyConfiguration(new ShoppingCartInfoProductItemEntityTypeConfiguration());
        }
    }
}