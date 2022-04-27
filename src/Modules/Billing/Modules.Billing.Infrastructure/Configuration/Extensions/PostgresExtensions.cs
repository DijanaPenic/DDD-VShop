using Microsoft.Extensions.DependencyInjection;

using VShop.SharedKernel.PostgresDb;
using VShop.SharedKernel.PostgresDb.Extensions;
using VShop.SharedKernel.Integration.DAL;
using VShop.SharedKernel.Subscriptions.DAL;
using VShop.Modules.Billing.Infrastructure.DAL;

namespace VShop.Modules.Billing.Infrastructure.Configuration.Extensions
{
    internal static class PostgresExtensions
    {
        public static void AddPostgres(this IServiceCollection services, PostgresOptions postgresOptions)
        {
            services.AddDbContextBuilder(postgresOptions.ConnectionString, typeof(BillingDbContext).Assembly);
            services.AddDbContext<BillingDbContext>();
            services.AddDbContext<IntegrationDbContext>();
            services.AddDbContext<SubscriptionDbContext>();
            services.AddUnitOfWork<BillingUnitOfWork>();
        }
    }
}