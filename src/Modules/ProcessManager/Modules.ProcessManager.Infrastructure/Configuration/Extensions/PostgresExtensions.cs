using Microsoft.Extensions.DependencyInjection;

using VShop.SharedKernel.PostgresDb;
using VShop.SharedKernel.Scheduler.DAL;
using VShop.SharedKernel.Subscriptions.DAL;
using VShop.SharedKernel.PostgresDb.Extensions;

namespace VShop.Modules.ProcessManager.Infrastructure.Configuration.Extensions;

internal static class PostgresExtensions
{
    public static void AddPostgres(this IServiceCollection services, PostgresOptions postgresOptions)
    {
        services.AddDbContextBuilder(postgresOptions.ConnectionString, typeof(ProcessManagerCompositionRoot).Assembly);
        services.AddDbContext<SubscriptionDbContext>();
        services.AddDbContext<SchedulerDbContext>();
    }
}