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
            
            builder.HasKey(sc => sc.EventId);
            builder.Property(sc => sc.EventTypeName).IsRequired();
            builder.Property(sc => sc.State).IsRequired();
            builder.Property(sc => sc.TimesSent).IsRequired();
            builder.Property(sc => sc.Content).IsRequired();
            builder.Property(sc => sc.TransactionId).IsRequired();
            builder.Property(sc => sc.DateCreated).IsRequired();
            builder.Property(sc => sc.DateUpdated).IsRequired();
        }
    }
}