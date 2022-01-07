using System;
using System.Threading;
using System.Threading.Tasks;

using VShop.SharedKernel.EventSourcing.ProcessManagers;

namespace VShop.SharedKernel.EventSourcing.Stores.Contracts
{
    public interface IProcessManagerStore<TProcess> where TProcess : ProcessManager
    {
        Task SaveAndPublishAsync
        (
            TProcess processManager,
            CancellationToken cancellationToken = default
        );

        Task<TProcess> LoadAsync
        (
            Guid processManagerId,
            Guid causationId,
            Guid correlationId,
            CancellationToken cancellationToken = default
        );
    }
}