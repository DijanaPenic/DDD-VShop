using System;
using System.Threading;
using System.Threading.Tasks;

using VShop.SharedKernel.EventSourcing.ProcessManagers;

namespace VShop.SharedKernel.EventSourcing.Repositories.Contracts
{
    public interface IProcessManagerRepository<TProcess>
        where TProcess : ProcessManager
    {
        Task SaveAsync(TProcess processManager, CancellationToken cancellationToken);
        Task<TProcess> LoadAsync(Guid processManagerId, CancellationToken cancellationToken);
    }
}