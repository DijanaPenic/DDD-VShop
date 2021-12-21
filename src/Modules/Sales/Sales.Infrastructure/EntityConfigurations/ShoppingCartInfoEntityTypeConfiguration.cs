using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using VShop.Modules.Sales.Infrastructure.Entities;

namespace VShop.Modules.Sales.Infrastructure.EntityConfigurations
{
    internal class ShoppingCartInfoEntityTypeConfiguration : IEntityTypeConfiguration<ShoppingCartInfo>
    {
        public void Configure(EntityTypeBuilder<ShoppingCartInfo> builder)
        {
            builder.ToTable("shopping_cart_info", SalesContext.ShoppingCartSchema);
            
            builder.HasKey(sc => sc.Id);
            builder.Property(sc => sc.CustomerId).IsRequired();
            builder.Property(sc => sc.Status).IsRequired();
            builder.Property(sc => sc.DateCreated).IsRequired();
            builder.Property(sc => sc.DateUpdated).IsRequired();
            
            builder.UseXminAsConcurrencyToken();
        }
    }
}