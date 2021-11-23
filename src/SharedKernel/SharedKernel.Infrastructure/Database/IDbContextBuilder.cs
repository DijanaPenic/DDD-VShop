using Microsoft.EntityFrameworkCore;

namespace VShop.SharedKernel.Infrastructure.Database
{
    public interface IDbContextBuilder
    {
        void ConfigureContext(DbContextOptionsBuilder optionsBuilder);
    }
}