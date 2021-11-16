using Quartz;
using Quartz.Spi;
using Quartz.Impl;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

using VShop.SharedKernel.Scheduler.Quartz.Jobs;
using VShop.SharedKernel.Scheduler.Quartz.Services;
using VShop.SharedKernel.Scheduler.Quartz.JobFactory;

namespace VShop.Modules.Sales.API.Infrastructure.Extensions
{
    public static class QuartzExtensions
    {
        public static IApplicationBuilder UseQuartz(this IApplicationBuilder app)
        {
            IScheduler scheduler = app.ApplicationServices.GetService<IScheduler>();
            scheduler?.Start().GetAwaiter().GetResult();

            return app;
        }

        public static IServiceCollection AddQuartzServices(this IServiceCollection services)
        {
            services.AddSingleton<IJobFactory, JobFactory>();
            services.AddSingleton(provider =>
            {
                StdSchedulerFactory schedulerFactory = new();
                
                IScheduler scheduler = schedulerFactory.GetScheduler().GetAwaiter().GetResult();
                scheduler.JobFactory = provider.GetService<IJobFactory>()!;

                return scheduler;
            });
            services.AddTransient<ICommandSchedulerService, CommandSchedulerService>();
            services.AddTransient<ProcessCommandJob>();

            return services;
        }
    }
}