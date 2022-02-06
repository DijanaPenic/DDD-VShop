using Microsoft.Extensions.DependencyInjection;

using VShop.Modules.Catalog.Infrastructure.DAL;
using VShop.SharedKernel.Integration.DAL;
using VShop.SharedKernel.PostgresDb;
using VShop.SharedKernel.PostgresDb.Extensions;
using VShop.SharedKernel.Subscriptions.DAL;

namespace VShop.Modules.Catalog.Infrastructure.Configuration.Extensions
{
    internal static class PostgresExtensions
    {
        public static void AddPostgres(this IServiceCollection services, string connectionString)
        {
            services.AddDbContextBuilder<CatalogDbContext>(connectionString);
            services.AddDbContext<CatalogDbContext>();
            services.AddDbContext<IntegrationDbContext>();
            services.AddDbContext<SubscriptionDbContext>();
            services.AddUnitOfWork<CatalogUnitOfWork>();
        }
    }
}