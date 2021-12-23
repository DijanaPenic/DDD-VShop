using Microsoft.Extensions.DependencyInjection;

using VShop.SharedKernel.PostgresDb;
using VShop.SharedKernel.Integration.Infrastructure;
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
                typeof(Startup).Assembly
            ));
            services.AddDbContext<BillingContext>();
            services.AddDbContext<IntegrationContext>();
            
            // Register the main dbContext provider.
            services.AddScoped<DbContextProvider>(provider => provider.GetService<BillingContext>);
        }
    }
}