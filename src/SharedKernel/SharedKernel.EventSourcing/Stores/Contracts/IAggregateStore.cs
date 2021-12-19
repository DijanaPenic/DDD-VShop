using System;
using System.Threading;
using System.Threading.Tasks;

using VShop.SharedKernel.Domain.ValueObjects;

namespace VShop.SharedKernel.EventSourcing.Stores.Contracts
{
    public interface IAggregateStore<TA>
    {
        Task SaveAndPublishAsync(TA aggregate, CancellationToken cancellationToken = default);
        Task SaveAsync(TA aggregate, CancellationToken cancellationToken = default);
        Task<TA> LoadAsync(EntityId aggregateId, Guid? causationId = default, Guid? correlationId = default, CancellationToken cancellationToken = default);
    }
}