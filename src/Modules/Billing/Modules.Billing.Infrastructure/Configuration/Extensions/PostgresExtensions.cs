using Microsoft.Extensions.DependencyInjection;

using VShop.SharedKernel.Integration.DAL;
using VShop.SharedKernel.Subscriptions.DAL;
using VShop.Modules.Billing.Infrastructure.DAL;
using VShop.SharedKernel.PostgresDb.Extensions;

namespace VShop.Modules.Billing.Infrastructure.Configuration.Extensions
{
    internal static class PostgresExtensions
    {
        public static void AddPostgres(this IServiceCollection services, string connectionString)
        {
            services.AddDbContextBuilder(connectionString, typeof(BillingDbContext).Assembly);
            services.AddDbContext<BillingDbContext>();
            services.AddDbContext<IntegrationDbContext>();
            services.AddDbContext<SubscriptionDbContext>();
            services.AddUnitOfWork<BillingUnitOfWork>();
        }
    }
}