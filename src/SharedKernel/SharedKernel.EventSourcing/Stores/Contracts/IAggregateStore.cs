using System.Threading;
using System.Threading.Tasks;

using VShop.SharedKernel.Domain.ValueObjects;
using VShop.SharedKernel.EventSourcing.Aggregates;

namespace VShop.SharedKernel.EventSourcing.Stores.Contracts
{
    public interface IAggregateStore<TAggregate> where TAggregate : AggregateRoot, new()
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
            CancellationToken cancellationToken = default
        );
    }
}