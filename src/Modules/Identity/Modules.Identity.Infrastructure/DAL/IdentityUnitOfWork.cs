using VShop.SharedKernel.PostgresDb;

namespace VShop.Modules.Identity.Infrastructure.DAL;

internal class IdentityUnitOfWork : PostgresUnitOfWork<IdentityDbContext>
{
    public IdentityUnitOfWork(IdentityDbContext dbContext) : base(dbContext)
    {
    }
}