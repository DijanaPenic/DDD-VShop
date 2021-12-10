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
            
            builder.HasKey(p => p.OrderId);
            builder.Property(p => p.Status).IsRequired();
            builder.Property(p => p.Error);
            builder.Property(p => p.DateCreated).IsRequired();
            builder.Property(p => p.DateUpdated).IsRequired();
        }
    }
}