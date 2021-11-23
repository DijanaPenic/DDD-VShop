using Microsoft.EntityFrameworkCore;

using VShop.SharedKernel.PostgresDb;
using VShop.SharedKernel.Infrastructure.Database;
using VShop.SharedKernel.Integration.Database.Entities;
using VShop.SharedKernel.Integration.Database.EntityConfigurations;

namespace VShop.SharedKernel.Integration.Database
{
    public class IntegrationContext : DbContextBase
    {
        public const string IntegrationSchema = "integration";
        private readonly IDbContextBuilder _contextBuilder;

        public DbSet<IntegrationEventLog> IntegrationEventLogs { get; set; }
        
        public IntegrationContext(IDbContextBuilder contextBuilder)
            => _contextBuilder = contextBuilder;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => _contextBuilder.ConfigureContext(optionsBuilder);

        protected override void OnModelCreating(ModelBuilder builder)
            => builder.ApplyConfiguration(new IntegrationEventLogEntityConfiguration());
    }
}