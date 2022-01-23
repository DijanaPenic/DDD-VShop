using Quartz;

using VShop.SharedKernel.Scheduler.DAL;
using VShop.SharedKernel.Scheduler.DAL.Entities;
using VShop.SharedKernel.Scheduler.Jobs;
using VShop.SharedKernel.Scheduler.Services.Contracts;
using VShop.SharedKernel.Infrastructure.Messaging;

namespace VShop.SharedKernel.Scheduler.Services
{
    public class SchedulerService : ISchedulerService
    {
        private readonly ISchedulerFactory _schedulerFactory;
        private readonly SchedulerDbContext _schedulerDbContext;

        public SchedulerService(ISchedulerFactory schedulerFactory, SchedulerDbContext schedulerDbContext)
        {
            _schedulerFactory = schedulerFactory;
            _schedulerDbContext = schedulerDbContext;
        }

        public async Task ScheduleMessageAsync
        (
            IScheduledMessage message,
            CancellationToken cancellationToken = default
        )
        {
            string messageId = message.Metadata.MessageId.Value;
            IScheduler scheduler = await _schedulerFactory.GetScheduler(cancellationToken);
            
            ITrigger existingTrigger = await scheduler.GetTrigger(new TriggerKey(messageId), cancellationToken);
            if(existingTrigger is not null) return;

            IJobDetail job = JobBuilder.Create<ProcessMessageJob>()
                .WithIdentity(messageId)
                .Build();

            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity(messageId)
                .StartAt(message.ScheduledTime.ToDateTimeOffset())
                .Build();
            
            await scheduler.ScheduleJob(job, trigger, cancellationToken);
            await LogScheduledMessageAsync(message, cancellationToken);
        }

        private Task LogScheduledMessageAsync
        (
            IScheduledMessage message,
            CancellationToken cancellationToken = default
        )
        {
            MessageLog messageLog = new(message);
            _schedulerDbContext.MessageLogs.Add(messageLog);

            return _schedulerDbContext.SaveChangesAsync(cancellationToken);
        }
    }
}