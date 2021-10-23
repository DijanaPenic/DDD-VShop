using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using VShop.Services.ShoppingCarts.Infrastructure.Entities;

namespace VShop.Services.ShoppingCarts.Infrastructure.EntityConfigurations
{
    internal class ShoppingCartProductItemEntityTypeConfiguration: IEntityTypeConfiguration<ShoppingCartInfoItem>
    {
        public void Configure(EntityTypeBuilder<ShoppingCartInfoItem> builder)
        {
            builder.ToTable("shopping_cart_details_product_item", ShoppingCartContext.DefaultSchema);
            
            builder.HasKey(bi => bi.Id);
            builder.Property(bi => bi.ProductId).IsRequired();
            builder.Property(bi => bi.Quantity).IsRequired();
            builder.Property(bi => bi.UnitPrice).IsRequired();
            builder.Property(bi => bi.DateCreatedUtc).IsRequired();
            builder.Property(bi => bi.DateUpdatedUtc).IsRequired();
            
            builder.HasOne(bi => bi.ShoppingCartInfo)
                   .WithMany(b => b.Items)
                   .HasForeignKey(bi => bi.ShoppingCartInfoId)
                   .IsRequired()
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}