using System.Threading;
using System.Threading.Tasks;

namespace VShop.SharedKernel.EventSourcing.Repositories
{
    public interface ICheckpointRepository
    {
        ValueTask<ulong?> LoadAsync(string subscriptionId, CancellationToken cancellationToken);
        ValueTask SaveAsync(string subscriptionId, ulong position, CancellationToken cancellationToken);
    }
}