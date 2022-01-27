using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using VShop.Modules.Catalog.Infrastructure.DAL.Entities;

namespace VShop.Modules.Catalog.Infrastructure.DAL.Configurations
{
    internal class CatalogCategoryEntityTypeConfiguration : IEntityTypeConfiguration<CatalogCategory>
    {
        public void Configure(EntityTypeBuilder<CatalogCategory> builder)
        {
            builder.ToTable("category", CatalogDbContext.CatalogSchema);
            
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Name).IsRequired();
            builder.Property(c => c.Description);
            builder.Property(c => c.IsDeleted).IsRequired().HasDefaultValue(false);
            builder.Property(c => c.DateCreated).IsRequired();
            builder.Property(c => c.DateUpdated).IsRequired();

            builder.UseXminAsConcurrencyToken();
        }
    }
}