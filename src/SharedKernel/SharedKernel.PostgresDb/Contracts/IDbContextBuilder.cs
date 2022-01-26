using Microsoft.EntityFrameworkCore;

namespace VShop.SharedKernel.PostgresDb.Contracts;

public interface IDbContextBuilder
{
    void ConfigureContext(DbContextOptionsBuilder optionsBuilder);
}