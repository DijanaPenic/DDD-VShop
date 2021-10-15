using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using VShop.Services.Basket.Infrastructure.Entities;

namespace VShop.Services.Basket.Infrastructure.EntityConfigurations
{
    internal class BasketDetailsEntityTypeConfiguration: IEntityTypeConfiguration<BasketDetails>
    {
        public void Configure(EntityTypeBuilder<BasketDetails> builder)
        {
            builder.ToTable("basket_details", BasketContext.DefaultSchema);
            
            builder.HasKey(b => b.Id);
            builder.Property(b => b.CustomerId).IsRequired();
            builder.Property(b => b.Status).IsRequired();
            builder.Property(b => b.Version).IsRequired();
        }
    }
}