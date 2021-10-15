using Microsoft.EntityFrameworkCore;

using VShop.Services.Basket.Infrastructure.Entities;
using VShop.Services.Basket.Infrastructure.EntityConfigurations;

namespace VShop.Services.Basket.Infrastructure
{
    public class BasketContext : DbContext
    {
        public const string DefaultSchema = "basket";

        public DbSet<BasketDetails> Baskets { get; set; }
        public DbSet<BasketDetailsProductItem> BasketItems { get; set; }
        
        public BasketContext(DbContextOptions<BasketContext> options) : base(options)
        {

        }
    
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new BasketDetailsEntityTypeConfiguration());
            builder.ApplyConfiguration(new BasketDetailsProductItemEntityTypeConfiguration());
        }
    }
}