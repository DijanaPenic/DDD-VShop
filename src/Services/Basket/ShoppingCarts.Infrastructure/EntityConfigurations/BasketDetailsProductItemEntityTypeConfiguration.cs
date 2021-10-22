using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using VShop.Services.Basket.Infrastructure.Entities;

namespace VShop.Services.Basket.Infrastructure.EntityConfigurations
{
    internal class BasketDetailsProductItemEntityTypeConfiguration: IEntityTypeConfiguration<BasketDetailsProductItem>
    {
        public void Configure(EntityTypeBuilder<BasketDetailsProductItem> builder)
        {
            builder.ToTable("basket_details_product_item", BasketContext.DefaultSchema);
            
            builder.HasKey(bi => bi.Id);
            builder.Property(bi => bi.ProductId).IsRequired();
            builder.Property(bi => bi.Quantity).IsRequired();
            builder.Property(bi => bi.UnitPrice).IsRequired();
            builder.Property(bi => bi.DateCreatedUtc).IsRequired();
            builder.Property(bi => bi.DateUpdatedUtc).IsRequired();
            
            builder.HasOne(bi => bi.BasketDetails)
                   .WithMany(b => b.ProductItems)
                   .HasForeignKey(bi => bi.BasketDetailsId)
                   .IsRequired()
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}