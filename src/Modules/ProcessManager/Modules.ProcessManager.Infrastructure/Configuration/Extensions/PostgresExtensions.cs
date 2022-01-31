using Microsoft.Extensions.DependencyInjection;

using VShop.SharedKernel.PostgresDb;
using VShop.SharedKernel.Scheduler.DAL;
using VShop.Modules.ProcessManager.Infrastructure.DAL;
using VShop.SharedKernel.Subscriptions.DAL;

namespace VShop.Modules.ProcessManager.Infrastructure.Configuration.Extensions;

internal static class PostgresExtensions
{
    public static void AddPostgres(this IServiceCollection services, string connectionString)
    {
        services.AddDbContextBuilder<ProcessManagerDbContext>(connectionString);
        services.AddDbContext<ProcessManagerDbContext>();
        services.AddDbContext<SubscriptionDbContext>();
        services.AddDbContext<SchedulerDbContext>();
    }
}