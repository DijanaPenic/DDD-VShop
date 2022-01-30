using System;
using System.Threading;
using System.Threading.Tasks;

using VShop.SharedKernel.EventSourcing.ProcessManagers;

namespace VShop.SharedKernel.EventSourcing.Stores.Contracts
{
    public interface IProcessManagerStore<TProcess> where TProcess : ProcessManager, new()
    {
        Task SaveAndPublishAsync
        (
            TProcess processManager,
            CancellationToken cancellationToken = default
        );

        Task<TProcess> LoadAsync
        (
            Guid processManagerId,
            CancellationToken cancellationToken = default
        );
    }
}