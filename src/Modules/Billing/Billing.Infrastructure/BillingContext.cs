using Microsoft.EntityFrameworkCore;

using VShop.SharedKernel.PostgresDb;
using VShop.SharedKernel.Infrastructure.Services.Contracts;
using VShop.Modules.Billing.Infrastructure.Entities;
using VShop.Modules.Billing.Infrastructure.EntityConfigurations;

namespace VShop.Modules.Billing.Infrastructure
{
    public class BillingContext : DbContextBase
    {
        public const string PaymentSchema = "payment";

        public DbSet<PaymentTransfer> Payments { get; set; }

        public BillingContext(IClockService clockService, IDbContextBuilder contextBuilder) 
            : base(clockService, contextBuilder) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => ContextBuilder.ConfigureContext(optionsBuilder);
        
        protected override void OnModelCreating(ModelBuilder builder)
            => builder.ApplyConfiguration(new PaymentEntityTypeConfiguration());
    }
}