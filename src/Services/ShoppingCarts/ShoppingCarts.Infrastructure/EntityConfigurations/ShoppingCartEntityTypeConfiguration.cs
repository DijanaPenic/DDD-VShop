using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using VShop.Services.ShoppingCarts.Infrastructure.Entities;

namespace VShop.Services.ShoppingCarts.Infrastructure.EntityConfigurations
{
    internal class ShoppingCartEntityTypeConfiguration: IEntityTypeConfiguration<ShoppingCartInfo>
    {
        public void Configure(EntityTypeBuilder<ShoppingCartInfo> builder)
        {
            builder.ToTable("shopping_cart_details", ShoppingCartContext.DefaultSchema);
            
            builder.HasKey(b => b.Id);
            builder.Property(b => b.CustomerId).IsRequired();
            builder.Property(b => b.Status).IsRequired();
            builder.Property(b => b.DateCreatedUtc).IsRequired();
            builder.Property(b => b.DateUpdatedUtc).IsRequired();
        }
    }
}