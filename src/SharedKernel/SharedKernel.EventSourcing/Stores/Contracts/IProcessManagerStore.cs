using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

using VShop.SharedKernel.EventSourcing.ProcessManagers;
using VShop.SharedKernel.Infrastructure.Messaging.Contracts;

namespace VShop.SharedKernel.EventSourcing.Stores.Contracts
{
    public interface IProcessManagerStore<TProcess> where TProcess : ProcessManager, new()
    {
        Task SaveAndPublishAsync
        (
            TProcess processManager,
            CancellationToken cancellationToken = default
        );

        Task PublishAsync
        (
            IEnumerable<IMessage> messages,
            CancellationToken cancellationToken = default
        );

        Task<TProcess> LoadAsync
        (
            Guid processManagerId,
            CancellationToken cancellationToken = default
        );
    }
}