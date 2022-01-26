using Microsoft.EntityFrameworkCore;

using VShop.SharedKernel.PostgresDb;
using VShop.SharedKernel.Integration.DAL.Entities;
using VShop.SharedKernel.Integration.DAL.EntityConfigurations;
using VShop.SharedKernel.Infrastructure.Services.Contracts;
using VShop.SharedKernel.PostgresDb.Contracts;

namespace VShop.SharedKernel.Integration.DAL
{
    public class IntegrationDbContext : DbContextBase
    {
        public const string IntegrationSchema = "integration";

        public DbSet<IntegrationEventLog> IntegrationEventLogs { get; set; }
        
        public IntegrationDbContext(IClockService clockService, IDbContextBuilder contextBuilder) 
            : base(clockService, contextBuilder) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => ContextBuilder.ConfigureContext(optionsBuilder);

        protected override void OnModelCreating(ModelBuilder builder)
            => builder.ApplyConfiguration(new IntegrationEventLogEntityConfiguration());
    }
}