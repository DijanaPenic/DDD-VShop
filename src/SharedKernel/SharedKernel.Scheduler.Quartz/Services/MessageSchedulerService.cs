using Quartz;
using System.Threading;
using System.Threading.Tasks;

using VShop.SharedKernel.Scheduler.Quartz.Jobs;
using VShop.SharedKernel.Scheduler.Quartz.Models;
using VShop.SharedKernel.Infrastructure.Helpers;

namespace VShop.SharedKernel.Scheduler.Quartz.Services
{
    public class MessageSchedulerService : IMessageSchedulerService
    {
        private readonly IScheduler _scheduler;

        public MessageSchedulerService(IScheduler scheduler)
            => _scheduler = scheduler;

        public async Task ScheduleCommandAsync(IScheduledCommand message, CancellationToken cancellationToken = default)
        {
            IJobDetail job = JobBuilder.Create<ProcessCommandJob>()
                .WithIdentity(SequentialGuid.Create().ToString())
                .UsingJobData(ProcessCommandJob.JobDataId, message.Id)
                .UsingJobData(ProcessCommandJob.JobDataBody, message.Body)
                .Build();

            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity(SequentialGuid.Create().ToString())
                .StartAt(message.ScheduledTime)
                .Build();

            await _scheduler.ScheduleJob(job, trigger, cancellationToken);
        }
        
        // Save scheduled message into the database
        // private Task SaveJob(ScheduledMessage message, CancellationToken cancellationToken = default)
        // {
        //     return Task.CompletedTask;
        // }
    }
}