using Quartz;
using System.Threading;
using System.Threading.Tasks;

using VShop.SharedKernel.Messaging;
using VShop.SharedKernel.Scheduler.Quartz.Jobs;
using VShop.SharedKernel.Scheduler.Infrastructure.Entities;
using VShop.SharedKernel.Infrastructure.Helpers;

using SchedulerContext = VShop.SharedKernel.Scheduler.Infrastructure.SchedulerContext;

namespace VShop.SharedKernel.Scheduler.Quartz.Services
{
    public class SchedulerService : ISchedulerService
    {
        private readonly ISchedulerFactory _schedulerFactory;
        private readonly SchedulerContext _schedulerContext;

        public SchedulerService(ISchedulerFactory schedulerFactory, SchedulerContext schedulerContext)
        {
            _schedulerFactory = schedulerFactory;
            _schedulerContext = schedulerContext;
        }

        public async Task ScheduleMessageAsync(IScheduledMessage message, CancellationToken cancellationToken = default)
        {
            await LogScheduledMessageAsync(message, cancellationToken);
                
            IJobDetail job = JobBuilder.Create<ProcessMessageJob>()
                .WithIdentity(SequentialGuid.Create().ToString())
                .UsingJobData(ProcessMessageJob.JobDataKey, message.MessageId.ToString())
                .Build();

            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity(SequentialGuid.Create().ToString())
                .StartAt(message.ScheduledTime.ToDateTimeOffset())
                .Build();

            IScheduler scheduler = await _schedulerFactory.GetScheduler(cancellationToken);

            await scheduler.ScheduleJob(job, trigger, cancellationToken);
        }
        
        private Task LogScheduledMessageAsync(IScheduledMessage message, CancellationToken cancellationToken = default)
        {
            MessageLog messageLog = new()
            {
                Id = message.MessageId,
                Body = message.Body,
                Status = MessageStatus.Scheduled,
                TypeName = message.TypeName,
                ScheduledTime = message.ScheduledTime
            };
            
            _schedulerContext.MessageLogs.Add(messageLog);

            return _schedulerContext.SaveChangesAsync(cancellationToken);
        }
    }
}