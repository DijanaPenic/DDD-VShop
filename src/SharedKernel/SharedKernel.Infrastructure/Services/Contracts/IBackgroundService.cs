using System.Threading;
using System.Threading.Tasks;

namespace VShop.SharedKernel.Infrastructure.Services.Contracts
{
    public interface IBackgroundService
    {
        Task StartAsync(CancellationToken cancellationToken);
        Task StopAsync(CancellationToken cancellationToken);
    }
}