using VShop.SharedKernel.Infrastructure.Messaging;
using VShop.SharedKernel.Infrastructure.Messaging.Contracts;

namespace VShop.SharedKernel.Scheduler.Services.Contracts
{
    public interface ISchedulerService
    {
        Task ScheduleMessageAsync
        (
            MessageEnvelope<IScheduledMessage> messageEnvelope,
            CancellationToken cancellationToken = default
        );
    }
}