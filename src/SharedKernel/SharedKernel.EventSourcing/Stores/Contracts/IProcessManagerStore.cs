using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

using VShop.SharedKernel.Messaging;
using VShop.SharedKernel.EventSourcing.ProcessManagers;

namespace VShop.SharedKernel.EventSourcing.Stores.Contracts
{
    public interface IProcessManagerStore<TProcess> where TProcess : ProcessManager, new()
    {
        Task SaveAndPublishAsync
        (
            TProcess processManager,
            Guid messageId,
            Guid correlationId,
            CancellationToken cancellationToken = default
        );

        Task PublishAsync
        (
            IEnumerable<IIdentifiedMessage<IMessage>> messages,
            CancellationToken cancellationToken = default
        );

        Task<TProcess> LoadAsync
        (
            Guid processManagerId,
            Guid messageId,
            CancellationToken cancellationToken = default
        );
    }
}