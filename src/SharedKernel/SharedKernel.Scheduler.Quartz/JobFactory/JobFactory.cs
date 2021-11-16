using Quartz;
using Quartz.Spi;
using System;
using Microsoft.Extensions.DependencyInjection;

namespace VShop.SharedKernel.Scheduler.Quartz.JobFactory
{
    public class JobFactory : IJobFactory
    {
        private readonly IServiceProvider _serviceProvider;
        
        public JobFactory(IServiceProvider serviceProvider)
            => _serviceProvider = serviceProvider;

        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
            => _serviceProvider.GetRequiredService(bundle.JobDetail.JobType) as IJob 
               ?? throw new InvalidOperationException();

        public void ReturnJob(IJob job) { }
    }
}