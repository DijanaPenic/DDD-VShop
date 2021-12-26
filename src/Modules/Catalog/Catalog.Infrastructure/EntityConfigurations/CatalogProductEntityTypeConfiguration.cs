using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using VShop.Modules.Catalog.Infrastructure.Entities;

namespace VShop.Modules.Catalog.Infrastructure.EntityConfigurations
{
    public class CatalogProductEntityTypeConfiguration : IEntityTypeConfiguration<CatalogProduct>
    {
        public void Configure(EntityTypeBuilder<CatalogProduct> builder)
        {
            builder.ToTable("product", CatalogContext.CatalogSchema);
            
            builder.HasKey(p => p.Id);
            builder.Property(p => p.CategoryId).IsRequired();
            builder.Property(p => p.SKU).IsRequired();
            builder.Property(p => p.Name).IsRequired();
            builder.Property(p => p.Description);
            builder.Property(p => p.Price).IsRequired();
            builder.Property(p => p.PictureUri);
            builder.Property(p => p.IsDeleted).IsRequired().HasDefaultValue(false);
            builder.Property(p => p.AvailableStock).IsRequired();
            builder.Property(p => p.MaxStockThreshold).IsRequired();
            builder.Property(p => p.DateCreated).IsRequired();
            builder.Property(p => p.DateUpdated).IsRequired();

            builder.HasCheckConstraint("positive_price", "`price` > 0");

            builder.HasOne(p => p.Category)
                .WithMany(cc => cc.Products)
                .HasForeignKey(p => p.CategoryId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
            
            builder.UseXminAsConcurrencyToken();
        }
    }
}