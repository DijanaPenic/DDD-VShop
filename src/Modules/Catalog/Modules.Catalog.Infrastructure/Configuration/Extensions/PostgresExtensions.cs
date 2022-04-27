using Microsoft.Extensions.DependencyInjection;

using VShop.SharedKernel.PostgresDb;
using VShop.SharedKernel.Integration.DAL;
using VShop.SharedKernel.PostgresDb.Extensions;
using VShop.SharedKernel.Subscriptions.DAL;
using VShop.Modules.Catalog.Infrastructure.DAL;

namespace VShop.Modules.Catalog.Infrastructure.Configuration.Extensions
{
    internal static class PostgresExtensions
    {
        public static void AddPostgres(this IServiceCollection services, PostgresOptions postgresOptions)
        {
            services.AddDbContextBuilder(postgresOptions.ConnectionString, typeof(CatalogDbContext).Assembly);
            services.AddDbContext<CatalogDbContext>();
            services.AddDbContext<IntegrationDbContext>();
            services.AddDbContext<SubscriptionDbContext>();
            services.AddUnitOfWork<CatalogUnitOfWork>();
        }
    }
}