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
            
            builder.HasKey(sc => sc.Id);
            builder.Property(sc => sc.Body).IsRequired();
            builder.Property(sc => sc.TypeName).IsRequired();
            builder.Property(sc => sc.ScheduledTime).IsRequired();
            builder.Property(sc => sc.Status).IsRequired();
            builder.Property(sc => sc.DateCreated).IsRequired();
            builder.Property(sc => sc.DateUpdated).IsRequired();
        }
    }
}