using VShop.SharedKernel.Infrastructure.Messaging.Contracts;

namespace VShop.SharedKernel.Scheduler.Services.Contracts
{
    public interface ISchedulerService
    {
        Task ScheduleMessageAsync
        (
            IScheduledMessage message,
            CancellationToken cancellationToken = default
        );
    }
}