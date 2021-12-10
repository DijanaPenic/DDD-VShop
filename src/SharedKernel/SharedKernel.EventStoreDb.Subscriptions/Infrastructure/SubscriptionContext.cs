using Microsoft.EntityFrameworkCore;

using VShop.SharedKernel.PostgresDb;
using VShop.SharedKernel.Infrastructure.Services.Contracts;
using VShop.SharedKernel.EventStoreDb.Subscriptions.Infrastructure.Entities;
using VShop.SharedKernel.EventStoreDb.Subscriptions.Infrastructure.EntityConfigurations;

namespace VShop.SharedKernel.EventStoreDb.Subscriptions.Infrastructure
{
    public class SubscriptionContext : DbContextBase
    {
        public const string SubscriptionSchema = "subscription";
        
        public DbSet<Checkpoint> Checkpoints { get; set; }

        public SubscriptionContext(IClockService clockService, IDbContextBuilder contextBuilder) 
            : base(clockService, contextBuilder) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => ContextBuilder.ConfigureContext(optionsBuilder);
    
        protected override void OnModelCreating(ModelBuilder builder)
            => builder.ApplyConfiguration(new CheckpointEntityTypeConfiguration());
    }
}