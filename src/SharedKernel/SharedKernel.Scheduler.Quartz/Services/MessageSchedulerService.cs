using Quartz;
using System.Threading;
using System.Threading.Tasks;

using VShop.SharedKernel.Scheduler.Quartz.Jobs;
using VShop.SharedKernel.Infrastructure.Helpers;
using VShop.SharedKernel.Infrastructure.Messaging;

namespace VShop.SharedKernel.Scheduler.Quartz.Services
{
    public class MessageSchedulerService : IMessageSchedulerService
    {
        private readonly IScheduler _scheduler;

        public MessageSchedulerService(IScheduler scheduler)
            => _scheduler = scheduler;

        public async Task ScheduleCommandAsync(IScheduledMessage scheduledCommand, CancellationToken cancellationToken = default)
        {
            IJobDetail job = JobBuilder.Create<ProcessCommandJob>()
                .WithIdentity(SequentialGuid.Create().ToString())
                .UsingJobData(ProcessCommandJob.JobDataBody, scheduledCommand.Message)
                .UsingJobData(ProcessCommandJob.JobDataType, scheduledCommand.Type)
                .Build();

            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity(SequentialGuid.Create().ToString())
                .StartAt(scheduledCommand.ScheduledTime)
                .Build();

            await _scheduler.ScheduleJob(job, trigger, cancellationToken);
        }
        
        // Save scheduled message into the database (will be needed to resume triggers)
        // private Task SaveJob(ScheduledMessage message, CancellationToken cancellationToken = default)
        // {
        //     return Task.CompletedTask;
        // }
    }
}