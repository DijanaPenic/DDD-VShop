using Microsoft.EntityFrameworkCore;

using VShop.SharedKernel.PostgresDb;
using VShop.SharedKernel.Infrastructure.Services.Contracts;
using VShop.Modules.Sales.Core.DAL.Entities;
using VShop.Modules.Sales.Core.DAL.Configurations;

namespace VShop.Modules.Sales.Core.DAL
{
    public class SalesDbContext : DbContextBase
    {
        public const string ShoppingCartSchema = "shopping_cart";
        public const string OrderSchema = "order";

        public DbSet<ShoppingCartInfo> ShoppingCarts { get; set; }
        public DbSet<ShoppingCartInfoItem> ShoppingCartItems { get; set; }

        public SalesDbContext(IClockService clockService, IDbContextBuilder contextBuilder) 
            : base(clockService, contextBuilder) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => ContextBuilder.ConfigureContext(optionsBuilder);
    
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new ShoppingCartInfoEntityTypeConfiguration());
            builder.ApplyConfiguration(new ShoppingCartInfoProductItemEntityTypeConfiguration());
        }
    }
}