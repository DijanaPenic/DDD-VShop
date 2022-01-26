using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

using VShop.SharedKernel.PostgresDb;
using VShop.SharedKernel.PostgresDb.Contracts;
using VShop.SharedKernel.Infrastructure.Services.Contracts;
using VShop.Modules.Billing.Infrastructure.DAL.Entities;

[assembly: InternalsVisibleTo("Database.DatabaseMigrator")]
namespace VShop.Modules.Billing.Infrastructure.DAL
{
    internal class BillingDbContext : DbContextBase
    {
        public const string PaymentSchema = "payment";

        public DbSet<Payment> Payments { get; set; }

        public BillingDbContext(IClockService clockService, IDbContextBuilder contextBuilder) 
            : base(clockService, contextBuilder) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => ContextBuilder.ConfigureContext(optionsBuilder);
        
        protected override void OnModelCreating(ModelBuilder builder)
            => builder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    }
}