using Quartz;
using System.Threading;
using System.Threading.Tasks;

using VShop.SharedKernel.Scheduler.Quartz.Jobs;
using VShop.SharedKernel.Scheduler.Quartz.Models;
using VShop.SharedKernel.Infrastructure.Helpers;

namespace VShop.SharedKernel.Scheduler.Quartz.Services
{
    public class CommandSchedulerService : ICommandSchedulerService
    {
        private readonly IScheduler _scheduler;

        public CommandSchedulerService(IScheduler scheduler)
            => _scheduler = scheduler;

        public async Task ScheduleCommandAsync(ScheduledCommand command, CancellationToken cancellationToken = default)
        {
            IJobDetail job = JobBuilder.Create<ProcessCommandJob>()
                .WithIdentity(SequentialGuid.Create().ToString())
                .UsingJobData(ProcessCommandJob.JobDataId, command.Id)
                .UsingJobData(ProcessCommandJob.JobDataBody, command.Body)
                .Build();

            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity(SequentialGuid.Create().ToString())
                .StartAt(command.ScheduledTime)
                .Build();

            await _scheduler.ScheduleJob(job, trigger, cancellationToken);
        }
        
        // Save scheduled message to database
        // private Task SaveJob(ScheduledMessage message, CancellationToken cancellationToken = default)
        // {
        //     return Task.CompletedTask;
        // }
    }
}