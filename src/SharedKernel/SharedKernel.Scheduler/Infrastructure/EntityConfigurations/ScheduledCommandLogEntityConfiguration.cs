using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using VShop.SharedKernel.Scheduler.Infrastructure.Entities;

namespace VShop.SharedKernel.Scheduler.Infrastructure.EntityConfigurations
{
    internal class ScheduledCommandLogEntityConfiguration : IEntityTypeConfiguration<MessageLog>
    {
        public void Configure(EntityTypeBuilder<MessageLog> builder)
        {
            builder.ToTable("message_log", SchedulerContext.SchedulerSchema);
            
            builder.HasKey(ml => ml.Id);
            builder.Property(ml => ml.TypeName).IsRequired();
            builder.Property(ml => ml.Body).IsRequired();
            builder.Property(ml => ml.Metadata).IsRequired();
            builder.Property(ml => ml.ScheduledTime).IsRequired();
            builder.Property(ml => ml.Status).IsRequired();
            builder.Property(ml => ml.DateCreated).IsRequired();
            builder.Property(ml => ml.DateUpdated).IsRequired();
            
            builder.UseXminAsConcurrencyToken();
        }
    }
}