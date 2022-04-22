using Microsoft.Extensions.DependencyInjection;

using VShop.SharedKernel.Scheduler.DAL;
using VShop.SharedKernel.Subscriptions.DAL;
using VShop.SharedKernel.PostgresDb.Extensions;

namespace VShop.Modules.ProcessManager.Infrastructure.Configuration.Extensions;

internal static class PostgresExtensions
{
    public static void AddPostgres(this IServiceCollection services, string connectionString)
    {
        services.AddDbContextBuilder(connectionString, typeof(ProcessManagerCompositionRoot).Assembly);
        services.AddDbContext<SubscriptionDbContext>();
        services.AddDbContext<SchedulerDbContext>();
    }
}