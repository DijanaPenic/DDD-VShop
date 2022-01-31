using Quartz;
using Microsoft.Extensions.DependencyInjection;

using VShop.SharedKernel.Scheduler.Jobs;
using VShop.SharedKernel.Scheduler.Services;
using VShop.SharedKernel.Scheduler.Services.Contracts;

namespace VShop.Modules.ProcessManager.Infrastructure.Configuration.Extensions;

internal static class SchedulerExtensions
{
    public static void AddScheduler(this IServiceCollection services, string connectionString)
    {
        services.Configure<QuartzOptions>(options =>
        {
            options.Scheduling.IgnoreDuplicates = true;
            options.Scheduling.OverWriteExistingData = true;
        });
        services.AddQuartz(configurator =>
        {
            configurator.SchedulerId = "Scheduler-ProcessManager";
            configurator.SchedulerName = "Quartz ASP.NET Core ProcessManager Scheduler";
            configurator.UseMicrosoftDependencyInjectionJobFactory();
            configurator.UseJobAutoInterrupt(options => { options.DefaultMaxRunTime = TimeSpan.FromMinutes(5); });
            configurator.UsePersistentStore(storeOptions =>
            {
                storeOptions.UseProperties = true;
                storeOptions.RetryInterval = TimeSpan.FromSeconds(15);
                storeOptions.UsePostgres(adoProviderOptions =>
                {
                    adoProviderOptions.ConnectionString = connectionString;
                    adoProviderOptions.TablePrefix = "scheduler.QRTZ_";
                });
                storeOptions.UseJsonSerializer();
            });
        });
        services.AddQuartzHostedService(options =>
        {
            // When shutting down we want jobs to complete gracefully.
            options.WaitForJobsToComplete = true;
        });
        services.AddTransient<ISchedulerService, SchedulerService>();
        services.AddTransient<IMessagingService, MessagingService>();
        services.AddTransient<ProcessMessageJob>();
    }
}