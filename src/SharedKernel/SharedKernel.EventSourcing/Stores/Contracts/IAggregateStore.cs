using System;
using System.Threading;
using System.Threading.Tasks;

using VShop.SharedKernel.Domain.ValueObjects;

namespace VShop.SharedKernel.EventSourcing.Stores.Contracts
{
    public interface IAggregateStore<TAggregate>
    {
        Task SaveAndPublishAsync
        (
            TAggregate aggregate,
            CancellationToken cancellationToken = default
        );

        Task SaveAsync
        (
            TAggregate aggregate,
            CancellationToken cancellationToken = default
        );

        Task<TAggregate> LoadAsync
        (
            EntityId aggregateId,
            Guid? causationId = default,
            Guid? correlationId = default,
            CancellationToken cancellationToken = default
        );
    }
}