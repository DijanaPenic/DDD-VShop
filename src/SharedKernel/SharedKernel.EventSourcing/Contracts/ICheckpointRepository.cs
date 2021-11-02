using System.Threading.Tasks;

namespace VShop.SharedKernel.EventSourcing.Contracts
{
    public interface ICheckpointRepository
    {
        Task<long?> GetCheckpointAsync();
        Task StoreCheckpointAsync(long? checkpoint);
    }
}