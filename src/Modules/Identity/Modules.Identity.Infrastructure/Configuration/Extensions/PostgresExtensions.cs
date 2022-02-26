using Microsoft.Extensions.DependencyInjection;

using VShop.SharedKernel.Integration.DAL;
using VShop.SharedKernel.PostgresDb.Extensions;
using VShop.Modules.Identity.Infrastructure.DAL;

namespace VShop.Modules.Identity.Infrastructure.Configuration.Extensions;

internal static class PostgresExtensions
{
    public static void AddPostgres(this IServiceCollection services, string connectionString)
    {
        services.AddDbContextBuilder<IdentityDbContext>(connectionString);
        services.AddDbContext<IdentityDbContext>();
        services.AddDbContext<IntegrationDbContext>();
        services.AddUnitOfWork<IdentityUnitOfWork>();
    }
}