using Microsoft.Extensions.DependencyInjection;

using VShop.SharedKernel.PostgresDb;
using VShop.SharedKernel.Integration.DAL;
using VShop.SharedKernel.Infrastructure.Services;
using VShop.SharedKernel.Subscriptions.DAL;
using VShop.Modules.Billing.Infrastructure;

namespace VShop.Modules.Billing.API.Infrastructure.Extensions
{
    public static class PostgresExtensions
    {
        public static void AddPostgresServices(this IServiceCollection services, string connectionString)
        {
            services.AddScoped<IDbContextBuilder>(_ => new DbContextBuilder
            (
                connectionString,
                typeof(BillingDbContext).Assembly
            ));
            services.AddDbContext<BillingDbContext>();
            services.AddDbContext<IntegrationDbContext>();
            services.AddDbContext<SubscriptionDbContext>();
            
            // Register the main dbContext provider.
            services.AddScoped<MainDbContextProvider>(provider => provider.GetService<BillingDbContext>);

            services.AddHostedService<DatabaseInitializerHostedService>();
        }
    }
}