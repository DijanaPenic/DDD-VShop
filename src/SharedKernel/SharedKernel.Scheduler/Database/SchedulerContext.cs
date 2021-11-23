using Microsoft.EntityFrameworkCore;

using VShop.SharedKernel.PostgresDb;
using VShop.SharedKernel.Scheduler.Database.Entities;
using VShop.SharedKernel.Scheduler.Database.EntityConfigurations;

// TODO - rename Database to Infrastructure
namespace VShop.SharedKernel.Scheduler.Database
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