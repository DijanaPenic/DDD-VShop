using Quartz;

using VShop.SharedKernel.Scheduler.DAL;
using VShop.SharedKernel.Scheduler.DAL.Entities;
using VShop.SharedKernel.Scheduler.Jobs;
using VShop.SharedKernel.Scheduler.Services.Contracts;
using VShop.SharedKernel.Infrastructure.Messaging.Contracts;

namespace VShop.SharedKernel.Scheduler.Services
{
    public class SchedulerService : ISchedulerService
    {
        private readonly ISchedulerFactory _schedulerFactory;
        private readonly SchedulerDbContext _schedulerDbContext;
        private readonly IMessageRegistry _messageRegistry;
        private readonly IMessageContextProvider _messageContextProvider;

        public SchedulerService
        (
            ISchedulerFactory schedulerFactory,
            SchedulerDbContext schedulerDbContext,
            IMessageRegistry messageRegistry,
            IMessageContextProvider messageContextProvider
        )
        {
            _schedulerFactory = schedulerFactory;
            _schedulerDbContext = schedulerDbContext;
            _messageRegistry = messageRegistry;
            _messageContextProvider = messageContextProvider;
        }

        public async Task ScheduleMessageAsync
        (
            IScheduledMessage message,
            CancellationToken cancellationToken = default
        )
        {
            IMessageContext messageContext = _messageContextProvider.Get(message);

            string messageId = messageContext.MessageId.ToString();
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
            await LogScheduledMessageAsync(message, messageContext, cancellationToken);
        }

        private Task LogScheduledMessageAsync
        (
            IScheduledMessage message,
            IMessageContext messageContext,
            CancellationToken cancellationToken = default
        )
        {
            ScheduledMessageLog scheduledMessageLog = new(message, messageContext, _messageRegistry);
            _schedulerDbContext.MessageLogs.Add(scheduledMessageLog);

            return _schedulerDbContext.SaveChangesAsync(cancellationToken);
        }
    }
}