using Quartz;
using System.Threading;
using System.Threading.Tasks;

using VShop.SharedKernel.Messaging;
using VShop.SharedKernel.Scheduler.Quartz.Jobs;
using VShop.SharedKernel.Scheduler.Database.Entities;
using VShop.SharedKernel.Infrastructure.Helpers;

using SchedulerContext = VShop.SharedKernel.Scheduler.Database.SchedulerContext;

namespace VShop.SharedKernel.Scheduler.Quartz.Services
{
    public class SchedulerService : ISchedulerService
    {
        private readonly ISchedulerFactory _schedulerFactory;
        private readonly SchedulerContext _dbContext;

        public SchedulerService(ISchedulerFactory schedulerFactory, SchedulerContext dbContext)
        {
            _schedulerFactory = schedulerFactory;
            _dbContext = dbContext;
        }

        public async Task ScheduleMessageAsync(IScheduledMessage message, CancellationToken cancellationToken = default)
        {
            await SaveJobAsync(message, cancellationToken);
                
            IJobDetail job = JobBuilder.Create<ProcessMessageJob>()
                .WithIdentity(SequentialGuid.Create().ToString())
                .UsingJobData(ProcessMessageJob.JobDataKey, message.MessageId.ToString())
                .Build();

            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity(SequentialGuid.Create().ToString())
                .StartAt(message.ScheduledTime)
                .Build();

            IScheduler scheduler = await _schedulerFactory.GetScheduler(cancellationToken);

            await scheduler.ScheduleJob(job, trigger, cancellationToken);
        }
        
        private Task SaveJobAsync(IScheduledMessage message, CancellationToken cancellationToken = default)
        {
            MessageLog messageLog = new()
            {
                Id = message.MessageId,
                Body = message.Body,
                Status = SchedulingStatus.Scheduled,
                TypeName = message.TypeName,
                ScheduledTime = message.ScheduledTime
            };
            
            _dbContext.MessageLogs.Add(messageLog);

            return _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}