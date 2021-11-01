using System.Threading.Tasks;

namespace VShop.SharedKernel.EventStore.Subscriptions.Contracts
{
    public interface IEventStoreSubscriptionManager
    {
        Task StartAsync();
        Task StopAsync();
    }
}