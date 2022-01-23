using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using VShop.SharedKernel.Domain.ValueObjects;
using VShop.SharedKernel.EventSourcing.Aggregates;
using VShop.SharedKernel.Infrastructure.Events.Contracts;

namespace VShop.SharedKernel.EventSourcing.Stores.Contracts
{
    public interface IAggregateStore<TAggregate> where TAggregate : AggregateRoot, new()
    {
        Task SaveAndPublishAsync
        (
            TAggregate aggregate,
            Guid messageId,
            Guid correlationId,
            CancellationToken cancellationToken = default
        );
        
        Task<IList<IBaseEvent>> SaveAsync
        (
            TAggregate aggregate,
            Guid causationId,
            Guid correlationId,
            CancellationToken cancellationToken = default
        );

        Task PublishAsync
        (
            IEnumerable<IBaseEvent> events,
            CancellationToken cancellationToken = default
        );

        Task<TAggregate> LoadAsync
        (
            EntityId aggregateId,
            Guid messageId,
            CancellationToken cancellationToken = default
        );
    }
}