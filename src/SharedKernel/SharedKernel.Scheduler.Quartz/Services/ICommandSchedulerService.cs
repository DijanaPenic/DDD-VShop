using System.Threading;
using System.Threading.Tasks;

using VShop.SharedKernel.Scheduler.Quartz.Models;

namespace VShop.SharedKernel.Scheduler.Quartz.Services
{
    public interface ICommandSchedulerService
    {
        Task ScheduleCommandAsync(ScheduledCommand message, CancellationToken cancellationToken = default);
    }
}