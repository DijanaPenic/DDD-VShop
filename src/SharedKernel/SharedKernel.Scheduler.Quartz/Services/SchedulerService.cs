using Quartz;
using System.Threading;
using System.Threading.Tasks;

using VShop.SharedKernel.Scheduler.Quartz.Jobs;
using VShop.SharedKernel.Infrastructure.Helpers;
using VShop.SharedKernel.Infrastructure.Messaging;
using VShop.SharedKernel.Scheduler.Database.Entities;

using SchedulerContext = VShop.SharedKernel.Scheduler.Database.SchedulerContext;

namespace VShop.SharedKernel.Scheduler.Quartz.Services
{
    public class SchedulerService : ISchedulerService
    {
        private readonly IScheduler _scheduler;
        private readonly SchedulerContext _dbContext;

        public SchedulerService(IScheduler scheduler, SchedulerContext dbContext)
        {
            _scheduler = scheduler;
            _dbContext = dbContext;
        }

        public async Task ScheduleMessageAsync(IScheduledMessage message, CancellationToken cancellationToken = default)
        {
            await SaveJobAsync(message, cancellationToken);
                
            IJobDetail job = JobBuilder.Create<ProcessMessageJob>()
                .WithIdentity(SequentialGuid.Create().ToString())
                .UsingJobData(ProcessMessageJob.JobDataKey, message.MessageId)
                .Build();

            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity(SequentialGuid.Create().ToString())
                .StartAt(message.ScheduledTime)
                .Build();

            await _scheduler.ScheduleJob(job, trigger, cancellationToken);
        }
        
        private Task SaveJobAsync(IScheduledMessage message, CancellationToken cancellationToken = default)
        {
            MessageLog messageLog = new()
            {
                Id = message.MessageId,
                Body = message.Body,
                Status = SchedulingStatus.Scheduled,
                RuntimeType = message.RuntimeType,
                MessageType = message.MessageType,
                ScheduledTime = message.ScheduledTime
            };
            
            _dbContext.MessageLogs.Add(messageLog);

            return _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}