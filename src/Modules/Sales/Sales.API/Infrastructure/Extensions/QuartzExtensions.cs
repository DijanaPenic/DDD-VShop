using System;
using Quartz;
using Microsoft.Extensions.DependencyInjection;

using VShop.SharedKernel.Scheduler.Quartz.Jobs;
using VShop.SharedKernel.Scheduler.Quartz.Services;

namespace VShop.Modules.Sales.API.Infrastructure.Extensions
{
    public static class QuartzExtensions
    {
        public static void AddQuartzServices(this IServiceCollection services, string connectionString)
        {
            services.Configure<QuartzOptions>(options =>
            {
                options.Scheduling.IgnoreDuplicates = true;
                options.Scheduling.OverWriteExistingData = true;
            });
            services.AddQuartz(configurator =>
            {
                configurator.SchedulerId = "Scheduler-Sales";
                configurator.SchedulerName = "Quartz ASP.NET Core Sales Scheduler";
                configurator.UseMicrosoftDependencyInjectionJobFactory();
                configurator.UseJobAutoInterrupt(options =>
                {
                    options.DefaultMaxRunTime = TimeSpan.FromMinutes(5);
                });
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
                // When shutting down we want jobs to complete gracefully
                options.WaitForJobsToComplete = true;
            });
            services.AddTransient<ISchedulerService, SchedulerService>();
            services.AddTransient<ProcessMessageJob>();
        }
    }
}