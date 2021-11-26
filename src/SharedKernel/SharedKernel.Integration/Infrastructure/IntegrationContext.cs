﻿using Microsoft.EntityFrameworkCore;

using VShop.SharedKernel.PostgresDb;
using VShop.SharedKernel.Integration.Infrastructure.Entities;
using VShop.SharedKernel.Integration.Infrastructure.EntityConfigurations;

namespace VShop.SharedKernel.Integration.Infrastructure
{
    public class IntegrationContext : DbContextBase
    {
        public const string IntegrationSchema = "integration";

        public DbSet<IntegrationEventLog> IntegrationEventLogs { get; set; }
        
        public IntegrationContext(IDbContextBuilder contextBuilder) : base(contextBuilder) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => ContextBuilder.ConfigureContext(optionsBuilder);

        protected override void OnModelCreating(ModelBuilder builder)
            => builder.ApplyConfiguration(new IntegrationEventLogEntityConfiguration());
    }
}