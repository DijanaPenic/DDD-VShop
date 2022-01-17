using Microsoft.Extensions.DependencyInjection;

using VShop.SharedKernel.PostgresDb;
using VShop.SharedKernel.Infrastructure.Services;
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
                typeof(SalesDbContext).Assembly
            ));
            services.AddDbContext<SalesDbContext>();
            services.AddDbContext<SchedulerDbContext>();
            services.AddDbContext<SubscriptionDbContext>();
            
            services.AddHostedService<DatabaseInitializerHostedService>();
        }
    }
}