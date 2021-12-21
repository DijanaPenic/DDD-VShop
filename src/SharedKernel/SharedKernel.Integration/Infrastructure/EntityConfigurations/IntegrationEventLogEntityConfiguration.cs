using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using VShop.SharedKernel.Integration.Infrastructure.Entities;

namespace VShop.SharedKernel.Integration.Infrastructure.EntityConfigurations
{
    public class IntegrationEventLogEntityConfiguration : IEntityTypeConfiguration<IntegrationEventLog>
    {
        public void Configure(EntityTypeBuilder<IntegrationEventLog> builder)
        {
            builder.ToTable("integration_event_log", IntegrationContext.IntegrationSchema);
            
            builder.HasKey(el => el.EventId);
            builder.Property(el => el.EventTypeName).IsRequired();
            builder.Property(el => el.State).IsRequired();
            builder.Property(el => el.TimesSent).IsRequired();
            builder.Property(el => el.Content).IsRequired();
            builder.Property(el => el.TransactionId).IsRequired();
            builder.Property(el => el.DateCreated).IsRequired();
            builder.Property(el => el.DateUpdated).IsRequired();
            
            builder.UseXminAsConcurrencyToken();
        }
    }
}