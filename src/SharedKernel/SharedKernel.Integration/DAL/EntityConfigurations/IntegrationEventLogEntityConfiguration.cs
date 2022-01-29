using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using VShop.SharedKernel.Integration.DAL.Entities;

namespace VShop.SharedKernel.Integration.DAL.EntityConfigurations
{
    public class IntegrationEventLogEntityConfiguration : IEntityTypeConfiguration<IntegrationEventLog>
    {
        public void Configure(EntityTypeBuilder<IntegrationEventLog> builder)
        {
            builder.ToTable("integration_event_queue", IntegrationDbContext.IntegrationSchema);
            
            builder.HasKey(el => el.Id);
            builder.Property(el => el.TypeName).IsRequired();
            builder.Property(el => el.State).IsRequired();
            builder.Property(el => el.TimesSent).IsRequired();
            builder.Property(el => el.Body).IsRequired();
            builder.Property(el => el.UserId).IsRequired();
            builder.Property(el => el.CausationId).IsRequired();
            builder.Property(el => el.CorrelationId).IsRequired();
            builder.Property(el => el.TransactionId).IsRequired();
            builder.Property(el => el.DateCreated).IsRequired();
            builder.Property(el => el.DateUpdated).IsRequired();
            
            builder.UseXminAsConcurrencyToken();
        }
    }
}