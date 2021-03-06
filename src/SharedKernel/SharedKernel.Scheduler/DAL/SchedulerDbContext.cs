using Microsoft.EntityFrameworkCore;

using VShop.SharedKernel.PostgresDb;
using VShop.SharedKernel.PostgresDb.Contracts;
using VShop.SharedKernel.Scheduler.DAL.Entities;
using VShop.SharedKernel.Infrastructure.Services.Contracts;

namespace VShop.SharedKernel.Scheduler.DAL
{
    public class SchedulerDbContext : DbContextBase
    {
        public const string SchedulerSchema = "scheduler";

        public DbSet<ScheduledMessageLog> MessageLogs { get; set; }

        public SchedulerDbContext(IClockService clockService, IDbContextBuilder contextBuilder) 
            : base(clockService, contextBuilder) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => ContextBuilder.ConfigureContext(optionsBuilder);
    
        protected override void OnModelCreating(ModelBuilder builder)
            => builder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    }
}