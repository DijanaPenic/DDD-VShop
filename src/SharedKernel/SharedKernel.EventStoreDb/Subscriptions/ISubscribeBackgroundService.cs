using System.Threading;
using System.Threading.Tasks;

namespace VShop.SharedKernel.EventStoreDb.Subscriptions
{
    public interface ISubscribeBackgroundService
    {
        Task StartAsync(CancellationToken cancellationToken);
        Task StopAsync(CancellationToken cancellationToken);
    }
}