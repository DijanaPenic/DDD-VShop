using Microsoft.EntityFrameworkCore;

using VShop.SharedKernel.PostgresDb;
using VShop.SharedKernel.Infrastructure.Services.Contracts;
using VShop.SharedKernel.EventStoreDb.Subscriptions.DAL.Entities;
using VShop.SharedKernel.EventStoreDb.Subscriptions.DAL.Configurations;

namespace VShop.SharedKernel.EventStoreDb.Subscriptions.DAL
{
    public class SubscriptionDbContext : DbContextBase
    {
        public const string SubscriptionSchema = "subscription";
        
        public DbSet<Checkpoint> Checkpoints { get; set; }
        public DbSet<MessageDeadLetterLog> MessageDeadLetterLogs { get; set; }

        public SubscriptionDbContext(IClockService clockService, IDbContextBuilder contextBuilder) 
            : base(clockService, contextBuilder) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => ContextBuilder.ConfigureContext(optionsBuilder);

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new CheckpointEntityTypeConfiguration());
            builder.ApplyConfiguration(new MessageDeadLetterLogEntityTypeConfiguration());
        }
    }
}