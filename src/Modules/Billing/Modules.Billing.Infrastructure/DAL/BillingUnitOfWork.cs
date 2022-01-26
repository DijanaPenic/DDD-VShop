using VShop.SharedKernel.PostgresDb;

namespace VShop.Modules.Billing.Infrastructure.DAL;

internal class BillingUnitOfWork : PostgresUnitOfWork<BillingDbContext>
{
    public BillingUnitOfWork(BillingDbContext dbContext) : base(dbContext)
    {
    }
}