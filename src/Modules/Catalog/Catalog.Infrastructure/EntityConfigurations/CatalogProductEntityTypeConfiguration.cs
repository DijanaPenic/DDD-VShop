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
            
            builder.HasKey(cp => cp.Id);
            builder.Property(cp => cp.CategoryId).IsRequired();
            builder.Property(cp => cp.Name).IsRequired();
            builder.Property(cp => cp.Description);
            builder.Property(cp => cp.Price).IsRequired();
            builder.Property(cp => cp.PictureUri);
            builder.Property(cp => cp.IsDeleted).IsRequired().HasDefaultValue(false);
            builder.Property(cp => cp.AvailableStock).IsRequired();
            builder.Property(cp => cp.MaxStockThreshold).IsRequired();
            builder.Property(cp => cp.DateCreated).IsRequired();
            builder.Property(cp => cp.DateUpdated).IsRequired();

            builder.HasCheckConstraint("positive_price", "`price` > 0");

            builder.HasOne(cp => cp.Category)
                .WithMany(cc => cc.Products)
                .HasForeignKey(cp => cp.CategoryId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
            
            builder.UseXminAsConcurrencyToken();
        }
    }
}