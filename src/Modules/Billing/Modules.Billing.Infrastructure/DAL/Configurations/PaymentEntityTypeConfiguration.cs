using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using VShop.Modules.Billing.Infrastructure.DAL.Entities;

namespace VShop.Modules.Billing.Infrastructure.DAL.Configurations
{
    internal class PaymentEntityTypeConfiguration : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            builder.ToTable("payment", BillingDbContext.PaymentSchema);
            
            builder.HasKey(pt => pt.Id);
            builder.Property(pt => pt.OrderId).IsRequired();
            builder.Property(pt => pt.Type).IsRequired();
            builder.Property(pt => pt.Status).IsRequired();
            builder.Property(pt => pt.Error);
            builder.Property(pt => pt.DateCreated).IsRequired();
            builder.Property(pt => pt.DateUpdated).IsRequired();
            
            builder.UseXminAsConcurrencyToken();
        }
    }
}