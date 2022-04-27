using Microsoft.Extensions.DependencyInjection;

using VShop.SharedKernel.PostgresDb;
using VShop.SharedKernel.PostgresDb.Extensions;
using VShop.SharedKernel.Integration.DAL;
using VShop.Modules.Identity.Infrastructure.DAL;

namespace VShop.Modules.Identity.Infrastructure.Configuration.Extensions;

internal static class PostgresExtensions
{
    public static void AddPostgres(this IServiceCollection services, PostgresOptions postgresOptions)
    {
        services.AddDbContextBuilder(postgresOptions.ConnectionString, typeof(IdentityDbContext).Assembly);
        services.AddDbContext<IdentityDbContext>();
        services.AddDbContext<IntegrationDbContext>();
        services.AddUnitOfWork<IdentityUnitOfWork>();
    }
}