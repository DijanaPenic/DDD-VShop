using Microsoft.Extensions.DependencyInjection;

using VShop.SharedKernel.PostgresDb;
using VShop.SharedKernel.Infrastructure.Services;
using VShop.SharedKernel.Integration.DAL;
using VShop.SharedKernel.Subscriptions.DAL;
using VShop.Modules.Catalog.Infrastructure;
using VShop.SharedKernel.PostgresDb.Contracts;

namespace VShop.Modules.Catalog.API.Infrastructure.Extensions
{
    public static class PostgresExtensions
    {
        public static void AddPostgresServices(this IServiceCollection services, string connectionString)
        {
            services.AddScoped<IDbContextBuilder>(_ => new DbContextBuilder
            (
                connectionString,
                typeof(CatalogDbContext).Assembly
            ));
            services.AddDbContext<CatalogDbContext>();
            services.AddDbContext<IntegrationDbContext>();
            services.AddDbContext<SubscriptionDbContext>();
            
            // Register the main dbContext provider.
            //services.AddScoped<MainDbContextProvider>(provider => provider.GetService<CatalogDbContext>);
            
            services.AddHostedService<DatabaseInitializerHostedService>();
        }
    }
}