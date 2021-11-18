using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using VShop.SharedKernel.Scheduler.Database.Entities;

namespace VShop.SharedKernel.Scheduler.Database.EntityConfigurations
{
    public class ScheduledCommandLogEntityConfiguration : IEntityTypeConfiguration<MessageLog>
    {
        public void Configure(EntityTypeBuilder<MessageLog> builder)
        {
            builder.ToTable("message_log", SchedulerContext.SchedulerSchema);
            
            builder.HasKey(sc => sc.Id);
            builder.Property(sc => sc.Body).IsRequired();
            builder.Property(sc => sc.RuntimeType).IsRequired();
            builder.Property(sc => sc.ScheduledTime).IsRequired();
            builder.Property(sc => sc.Status).IsRequired();
            builder.Property(sc => sc.DateCreatedUtc).IsRequired();
            builder.Property(sc => sc.DateUpdatedUtc).IsRequired();
        }
    }
}