using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

using VShop.SharedKernel.PostgresDb;
using VShop.SharedKernel.Integration.Database;
using VShop.SharedKernel.Infrastructure.Database;
using VShop.Modules.Billing.Infrastructure;

namespace VShop.Modules.Billing.API.Infrastructure.Extensions
{
    public static class PostgresExtensions
    {
        public static void AddPostgresServices(this IServiceCollection services, string connectionString)
        {
            services.AddScoped<IDbContextBuilder>(_ => new PostgresDbContextBuilder
            (
                connectionString,
                Assembly.GetExecutingAssembly())
            );
            services.AddDbContext<BillingContext>();
            services.AddDbContext<IntegrationContext>();
        }
    }
}