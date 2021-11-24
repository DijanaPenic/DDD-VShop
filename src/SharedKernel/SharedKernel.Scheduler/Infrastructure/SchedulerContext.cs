using Microsoft.EntityFrameworkCore;

using VShop.SharedKernel.PostgresDb;
using VShop.SharedKernel.Scheduler.Infrastructure.Entities;
using VShop.SharedKernel.Scheduler.Infrastructure.EntityConfigurations;

namespace VShop.SharedKernel.Scheduler.Infrastructure
{
    public class SchedulerContext : DbContextBase
    {
        public const string SchedulerSchema = "scheduler";

        public DbSet<MessageLog> MessageLogs { get; set; }

        public SchedulerContext(DbContextOptions<SchedulerContext> options) : base(options) { }
    
        protected override void OnModelCreating(ModelBuilder builder)
            => builder.ApplyConfiguration(new ScheduledCommandLogEntityConfiguration());
    }
}