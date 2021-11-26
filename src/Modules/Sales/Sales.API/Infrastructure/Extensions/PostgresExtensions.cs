using Microsoft.Extensions.DependencyInjection;

using VShop.SharedKernel.PostgresDb;
using VShop.SharedKernel.Scheduler.Infrastructure;
using VShop.SharedKernel.EventStoreDb.Subscriptions.Infrastructure;
using VShop.Modules.Sales.Infrastructure;

namespace VShop.Modules.Sales.API.Infrastructure.Extensions
{
    public static class PostgresExtensions
    {
        public static void AddPostgresServices(this IServiceCollection services, string connectionString)
        {
            services.AddScoped<IDbContextBuilder>(_ => new DbContextBuilder
            (
                connectionString,
                typeof(Startup).Assembly
            ));
            services.AddDbContext<SalesContext>();
            services.AddDbContext<SchedulerContext>();
            services.AddDbContext<SubscriptionContext>();
        }
    }
}