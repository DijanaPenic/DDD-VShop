using Microsoft.EntityFrameworkCore;

using VShop.SharedKernel.PostgresDb;
using VShop.Modules.Billing.Infrastructure.Entities;
using VShop.Modules.Billing.Infrastructure.EntityConfigurations;

namespace VShop.Modules.Billing.Infrastructure
{
    public class BillingContext : DbContextBase
    {
        private readonly IDbContextBuilder _contextBuilder;
        
        public const string PaymentSchema = "payment";

        public DbSet<PaymentTransfer> Payments { get; set; }

        public BillingContext(IDbContextBuilder contextBuilder)
            => _contextBuilder = contextBuilder;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => _contextBuilder.ConfigureContext(optionsBuilder);
        
        protected override void OnModelCreating(ModelBuilder builder)
            => builder.ApplyConfiguration(new PaymentEntityTypeConfiguration());
    }
}