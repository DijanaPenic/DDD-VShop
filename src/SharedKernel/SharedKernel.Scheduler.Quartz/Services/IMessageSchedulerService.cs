using System.Threading;
using System.Threading.Tasks;

using VShop.SharedKernel.Infrastructure.Messaging;

namespace VShop.SharedKernel.Scheduler.Quartz.Services
{
    public interface IMessageSchedulerService
    {
        Task ScheduleCommandAsync(IScheduledMessage message, CancellationToken cancellationToken = default);
    }
}