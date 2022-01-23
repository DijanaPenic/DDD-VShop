using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using VShop.SharedKernel.Scheduler.DAL.Entities;

namespace VShop.SharedKernel.Scheduler.DAL.EntityConfigurations
{
    internal class ScheduledCommandLogEntityConfiguration : IEntityTypeConfiguration<MessageLog>
    {
        public void Configure(EntityTypeBuilder<MessageLog> builder)
        {
            builder.ToTable("message_log", SchedulerDbContext.SchedulerSchema);
            
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