using Quartz;
using System.Threading;
using System.Threading.Tasks;

using VShop.SharedKernel.Messaging;
using VShop.SharedKernel.Scheduler.Jobs;
using VShop.SharedKernel.Scheduler.Services.Contracts;
using VShop.SharedKernel.Scheduler.Infrastructure.Entities;

using SchedulerContext = VShop.SharedKernel.Scheduler.Infrastructure.SchedulerContext;

namespace VShop.SharedKernel.Scheduler.Services
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
            _schedulerContext.MessageLogs.Add(messageLog);

            return _schedulerContext.SaveChangesAsync(cancellationToken);
        }
    }
}