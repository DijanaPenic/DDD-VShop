using System.Threading;
using System.Threading.Tasks;

using VShop.SharedKernel.Scheduler.Quartz.Models;

namespace VShop.SharedKernel.Scheduler.Quartz.Services
{
    public interface IMessageSchedulerService
    {
        Task ScheduleCommandAsync(IScheduledCommand message, CancellationToken cancellationToken = default);
    }
}