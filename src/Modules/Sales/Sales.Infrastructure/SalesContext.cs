using Microsoft.EntityFrameworkCore;

using VShop.SharedKernel.PostgresDb;
using VShop.Modules.Sales.Infrastructure.Entities;
using VShop.Modules.Sales.Infrastructure.EntityConfigurations;

namespace VShop.Modules.Sales.Infrastructure
{
    public class SalesContext : ApplicationDbContextBase
    {
        public const string ShoppingCartSchema = "shopping_cart";
        public const string OrderSchema = "order";

        public DbSet<ShoppingCartInfo> ShoppingCarts { get; set; }
        public DbSet<ShoppingCartInfoItem> ShoppingCartItems { get; set; }

        public SalesContext(DbContextOptions<SalesContext> options) : base(options)
        {

        }
    
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new ShoppingCartInfoEntityTypeConfiguration());
            builder.ApplyConfiguration(new ShoppingCartInfoProductItemEntityTypeConfiguration());
        }
    }
}