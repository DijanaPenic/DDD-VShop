using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using VShop.Modules.Sales.Infrastructure.Entities;

namespace VShop.Modules.Sales.Infrastructure.EntityConfigurations
{
    public class OrderFulfillmentProcessEntityTypeConfiguration : IEntityTypeConfiguration<OrderFulfillmentProcess>
    {
        public void Configure(EntityTypeBuilder<OrderFulfillmentProcess> builder)
        {
            builder.ToTable("order_fulfillment_process", SalesContext.OrderSchema);
            
            builder.HasKey(ofp => ofp.ShoppingCartId);
            builder.Property(ofp => ofp.OrderId).IsRequired();
            builder.Property(ofp => ofp.Status).IsRequired();
            builder.Property(ofp => ofp.Description);
            builder.Property(ofp => ofp.DateCreatedUtc).IsRequired();
            builder.Property(ofp => ofp.DateUpdatedUtc).IsRequired();
        }
    }
}