using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using VShop.SharedKernel.Messaging;
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

        Task SaveAsync
        (
            TProcess processManager,
            CancellationToken cancellationToken = default
        );

        Task<TProcess> LoadAsync
        (
            Guid processManagerId,
            Guid? causationId = default,
            Guid? correlationId = default,
            CancellationToken cancellationToken = default
        );

        Task<IReadOnlyList<IMessage>> LoadInboxAsync
        (
            Guid processManagerId,
            CancellationToken cancellationToken = default
        );

        Task<IReadOnlyList<IMessage>> LoadOutboxAsync
        (
            Guid processManagerId,
            CancellationToken cancellationToken = default
        );
    }
}