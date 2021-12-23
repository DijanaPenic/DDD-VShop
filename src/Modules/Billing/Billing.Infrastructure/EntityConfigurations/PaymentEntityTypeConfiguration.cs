using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using VShop.Modules.Billing.Infrastructure.Entities;

namespace VShop.Modules.Billing.Infrastructure.EntityConfigurations
{
    internal class PaymentEntityTypeConfiguration : IEntityTypeConfiguration<PaymentTransfer>
    {
        public void Configure(EntityTypeBuilder<PaymentTransfer> builder)
        {
            builder.ToTable("payment_transfer", BillingContext.PaymentSchema);
            
            builder.HasKey(pt => pt.Id);
            builder.Property(pt => pt.OrderId).IsRequired();
            builder.Property(pt => pt.Status).IsRequired();
            builder.Property(pt => pt.Error);
            builder.Property(pt => pt.DateCreated).IsRequired();
            builder.Property(pt => pt.DateUpdated).IsRequired();
            
            builder.UseXminAsConcurrencyToken();
        }
    }
}