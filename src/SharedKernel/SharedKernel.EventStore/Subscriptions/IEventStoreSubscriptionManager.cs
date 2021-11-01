using System.Threading.Tasks;

namespace VShop.SharedKernel.EventStore.Subscriptions
{
    public interface IEventStoreSubscriptionManager
    {
        Task StartAsync();
        Task StopAsync();
    }
}