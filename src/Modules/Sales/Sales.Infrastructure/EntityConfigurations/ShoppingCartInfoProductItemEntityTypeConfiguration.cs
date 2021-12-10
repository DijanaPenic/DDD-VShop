using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using VShop.Modules.Sales.Infrastructure.Entities;

namespace VShop.Modules.Sales.Infrastructure.EntityConfigurations
{
    internal class ShoppingCartInfoProductItemEntityTypeConfiguration : IEntityTypeConfiguration<ShoppingCartInfoItem>
    {
        public void Configure(EntityTypeBuilder<ShoppingCartInfoItem> builder)
        {
            builder.ToTable("shopping_cart_info_product_item", SalesContext.ShoppingCartSchema);
            
            builder.HasKey(sci => sci.ProductId);
            builder.Property(sci => sci.Quantity).IsRequired();
            builder.Property(sci => sci.UnitPrice).IsRequired();
            builder.Property(sci => sci.DateCreated).IsRequired();
            builder.Property(sci => sci.DateUpdated).IsRequired();
            
            builder.HasOne(sci => sci.ShoppingCartInfo)
                   .WithMany(sc => sc.Items)
                   .HasForeignKey(sci => sci.ShoppingCartInfoId)
                   .IsRequired()
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}