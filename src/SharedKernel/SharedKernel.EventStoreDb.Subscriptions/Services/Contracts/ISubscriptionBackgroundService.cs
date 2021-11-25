using System.Threading;
using System.Threading.Tasks;

namespace VShop.SharedKernel.EventStoreDb.Subscriptions.Services.Contracts
{
    public interface ISubscriptionBackgroundService
    {
        Task StartAsync(CancellationToken cancellationToken);
        Task StopAsync(CancellationToken cancellationToken);
    }
}