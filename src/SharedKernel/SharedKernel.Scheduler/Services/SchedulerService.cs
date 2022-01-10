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
            IIdentifiedMessage<IScheduledMessage> message,
            CancellationToken cancellationToken = default
        )
        {
            await LogScheduledMessageAsync(message, cancellationToken);
                
            IJobDetail job = JobBuilder.Create<ProcessMessageJob>()
                .WithIdentity(message.Metadata.MessageId.ToString())
                .Build();

            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity(message.Metadata.MessageId.ToString())
                .StartAt(message.Data.ScheduledTime.ToDateTimeOffset())
                .Build();

            IScheduler scheduler = await _schedulerFactory.GetScheduler(cancellationToken);

            await scheduler.ScheduleJob(job, trigger, cancellationToken);
        }

        private Task LogScheduledMessageAsync
        (
            IIdentifiedMessage<IScheduledMessage> message,
            CancellationToken cancellationToken = default
        )
        {
            MessageLog messageLog = new()
            {
                Id = message.Metadata.MessageId,
                Body = message.Data.Body,
                Status = MessageStatus.Scheduled,
                TypeName = message.Data.TypeName,
                ScheduledTime = message.Data.ScheduledTime
            };
            
            _schedulerContext.MessageLogs.Add(messageLog);

            return _schedulerContext.SaveChangesAsync(cancellationToken);
        }
    }
}