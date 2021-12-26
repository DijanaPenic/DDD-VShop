using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using VShop.Modules.Catalog.Infrastructure.Entities;

namespace VShop.Modules.Catalog.Infrastructure.EntityConfigurations
{
    public class CatalogCategoryEntityTypeConfiguration : IEntityTypeConfiguration<CatalogCategory>
    {
        public void Configure(EntityTypeBuilder<CatalogCategory> builder)
        {
            builder.ToTable("product", CatalogContext.CatalogSchema);
            
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Name).IsRequired();
            builder.Property(c => c.IsDeleted).IsRequired().HasDefaultValue(false);
            builder.Property(c => c.DateCreated).IsRequired();
            builder.Property(c => c.DateUpdated).IsRequired();

            builder.UseXminAsConcurrencyToken();
        }
    }
}