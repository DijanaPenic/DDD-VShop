using System;
using System.Threading;
using System.Threading.Tasks;

namespace VShop.SharedKernel.Scheduler.Services
{
    public interface IMessagingService
    {
        Task SendMessageAsync(Guid commandId, CancellationToken cancellationToken);
    }
}