using Microsoft.EntityFrameworkCore;

namespace VShop.SharedKernel.Infrastructure
{
    public interface IDbContextBuilder
    {
        void ConfigureContext(DbContextOptionsBuilder optionsBuilder);
    }
}