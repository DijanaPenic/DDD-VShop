using VShop.SharedKernel.PostgresDb;

namespace VShop.Modules.Catalog.Infrastructure.DAL;

internal class CatalogUnitOfWork : PostgresUnitOfWork<CatalogDbContext>
{
    public CatalogUnitOfWork(CatalogDbContext dbContext) : base(dbContext)
    {
    }
}