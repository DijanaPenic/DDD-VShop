using System.Threading.Tasks;

namespace VShop.SharedKernel.EventStore.Repositories.Contracts
{
    public interface IEventStoreCheckpointRepository
    {
        Task<long?> GetCheckpointAsync();
        Task StoreCheckpointAsync(long? checkpoint);
    }
}