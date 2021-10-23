using Microsoft.EntityFrameworkCore;

using VShop.SharedKernel.PostgresDb;
using VShop.Services.ShoppingCarts.Infrastructure.Entities;
using VShop.Services.ShoppingCarts.Infrastructure.EntityConfigurations;

namespace VShop.Services.ShoppingCarts.Infrastructure
{
    public class ShoppingCartContext : ApplicationDbContextBase
    {
        public const string DefaultSchema = "shopping_cart";

        public DbSet<ShoppingCartInfo> ShoppingCarts { get; set; }
        public DbSet<ShoppingCartInfoItem> ShoppingCartItems { get; set; }
        
        public ShoppingCartContext(DbContextOptions<ShoppingCartContext> options) : base(options)
        {

        }
    
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new ShoppingCartEntityTypeConfiguration());
            builder.ApplyConfiguration(new ShoppingCartProductItemEntityTypeConfiguration());
        }
    }
}