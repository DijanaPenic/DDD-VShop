using System.Runtime.CompilerServices;
using Microsoft.Extensions.DependencyInjection;

using VShop.SharedKernel.PostgresDb;
using VShop.SharedKernel.Scheduler.DAL;
using VShop.SharedKernel.EventStoreDb.Subscriptions.DAL;
using VShop.Modules.Sales.Infrastructure.DAL;

[assembly: InternalsVisibleTo("VShop.Modules.Sales.API")]
namespace VShop.Modules.Sales.Infrastructure.Configuration.Extensions;

internal static class PostgresExtensions
{
    public static void AddPostgres(this IServiceCollection services, string connectionString)
    {
        services.AddScoped<IDbContextBuilder>(_ => new DbContextBuilder
        (
            connectionString,
            typeof(SalesDbContext).Assembly
        ));
        services.AddDbContext<SalesDbContext>();
        services.AddDbContext<SchedulerDbContext>();
        services.AddDbContext<SubscriptionDbContext>();
    }
}