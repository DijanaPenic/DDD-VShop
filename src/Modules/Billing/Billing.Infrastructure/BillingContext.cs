using Microsoft.EntityFrameworkCore;

using VShop.SharedKernel.PostgresDb;
using VShop.Modules.Billing.Infrastructure.Entities;
using VShop.Modules.Billing.Infrastructure.EntityConfigurations;

namespace VShop.Modules.Billing.Infrastructure
{
    public class BillingContext : ApplicationDbContextBase
    {
        public const string PaymentSchema = "payment";

        public DbSet<PaymentTransfer> Payments { get; set; }

        public BillingContext(DbContextOptions<BillingContext> options) : base(options)
        {

        }
    
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new PaymentEntityTypeConfiguration());
        }
    }
}