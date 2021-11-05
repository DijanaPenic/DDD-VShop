using System.Threading.Tasks;

namespace VShop.SharedKernel.EventSourcing.Repositories
{
    public interface ICheckpointRepository
    {
        Task<long?> GetCheckpointAsync();
        Task StoreCheckpointAsync(long? checkpoint);
    }
}