using System.Threading.Tasks;

namespace VShop.SharedKernel.EventStore
{
    public interface IEventStoreCheckpointRepository
    {
        Task<long?> GetCheckpointAsync();
        Task StoreCheckpointAsync(long? checkpoint);
    }
}