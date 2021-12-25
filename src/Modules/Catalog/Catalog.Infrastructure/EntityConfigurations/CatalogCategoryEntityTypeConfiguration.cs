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
            
            builder.HasKey(cc => cc.Id);
            builder.Property(cc => cc.Name).IsRequired();
            builder.Property(cc => cc.IsDeleted).IsRequired().HasDefaultValue(false);
            builder.Property(cc => cc.DateCreated).IsRequired();
            builder.Property(cc => cc.DateUpdated).IsRequired();

            builder.UseXminAsConcurrencyToken();
        }
    }
}