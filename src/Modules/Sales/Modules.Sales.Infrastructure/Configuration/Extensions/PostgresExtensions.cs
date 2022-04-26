using Microsoft.Extensions.DependencyInjection;

using VShop.SharedKernel.PostgresDb;
using VShop.SharedKernel.Scheduler.DAL;
using VShop.SharedKernel.Subscriptions.DAL;
using VShop.Modules.Sales.Infrastructure.DAL;
using VShop.SharedKernel.PostgresDb.Extensions;

namespace VShop.Modules.Sales.Infrastructure.Configuration.Extensions;

internal static class PostgresExtensions
{
    public static void AddPostgres(this IServiceCollection services, PostgresOptions postgresOptions)
    {
        services.AddDbContextBuilder(postgresOptions.ConnectionString, typeof(SalesDbContext).Assembly);
        services.AddDbContext<SalesDbContext>();
        services.AddDbContext<SubscriptionDbContext>();
        services.AddUnitOfWork<SalesUnitOfWork>();
    }
}