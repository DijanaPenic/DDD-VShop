using VShop.SharedKernel.PostgresDb;

namespace VShop.Modules.Sales.Infrastructure.DAL;

internal class SalesUnitOfWork : PostgresUnitOfWork<SalesDbContext>
{
    public SalesUnitOfWork(SalesDbContext dbContext) : base(dbContext)
    {
    }
}