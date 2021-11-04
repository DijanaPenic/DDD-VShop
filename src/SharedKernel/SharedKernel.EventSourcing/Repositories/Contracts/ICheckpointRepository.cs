using System.Threading.Tasks;

namespace VShop.SharedKernel.EventSourcing.Repositories.Contracts
{
    public interface ICheckpointRepository
    {
        Task<long?> GetCheckpointAsync();
        Task StoreCheckpointAsync(long? checkpoint);
    }
}