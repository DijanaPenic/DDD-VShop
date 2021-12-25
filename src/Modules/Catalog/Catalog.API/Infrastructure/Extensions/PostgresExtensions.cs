using Microsoft.Extensions.DependencyInjection;

using VShop.SharedKernel.PostgresDb;
using VShop.SharedKernel.Integration.Infrastructure;
using VShop.Modules.Catalog.Infrastructure;

namespace VShop.Modules.Catalog.API.Infrastructure.Extensions
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
            services.AddDbContext<CatalogContext>();
            services.AddDbContext<IntegrationContext>();
            
            // Register the main dbContext provider.
            services.AddScoped<MainDbContextProvider>(provider => provider.GetService<CatalogContext>);
        }
    }
}