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
            
            builder.HasKey(sci => sci.Id);
            builder.Property(sci => sci.ProductId).IsRequired();
            builder.Property(sci => sci.Quantity).IsRequired();
            builder.Property(sci => sci.UnitPrice).IsRequired();
            builder.Property(sci => sci.DateCreatedUtc).IsRequired();
            builder.Property(sci => sci.DateUpdatedUtc).IsRequired();
            
            builder.HasOne(sci => sci.ShoppingCartInfo)
                   .WithMany(sc => sc.Items)
                   .HasForeignKey(sci => sci.ShoppingCartInfoId)
                   .IsRequired()
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}