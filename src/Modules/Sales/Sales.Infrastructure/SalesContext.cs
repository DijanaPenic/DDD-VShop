using Microsoft.EntityFrameworkCore;

using VShop.SharedKernel.PostgresDb;
using VShop.Services.Sales.Infrastructure.Entities;
using VShop.Services.Sales.Infrastructure.EntityConfigurations;

namespace VShop.Services.Sales.Infrastructure
{
    public class SalesContext : ApplicationDbContextBase
    {
        public const string ShoppingCartSchema = "shopping_cart";

        public DbSet<ShoppingCartInfo> Sales { get; set; }
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