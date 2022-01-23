using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using VShop.Modules.Sales.Core.DAL.Entities;

namespace VShop.Modules.Sales.Core.DAL.Configurations
{
    internal class ShoppingCartInfoEntityTypeConfiguration : IEntityTypeConfiguration<ShoppingCartInfo>
    {
        public void Configure(EntityTypeBuilder<ShoppingCartInfo> builder)
        {
            builder.ToTable("shopping_cart_info", SalesDbContext.ShoppingCartSchema);
            
            builder.HasKey(sc => sc.Id);
            builder.Property(sc => sc.CustomerId).IsRequired();
            builder.Property(sc => sc.Status).IsRequired();
            builder.Property(sc => sc.DateCreated).IsRequired();
            builder.Property(sc => sc.DateUpdated).IsRequired();
            
            builder.UseXminAsConcurrencyToken();
        }
    }
}