using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

using VShop.SharedKernel.Messaging.Events;
using VShop.SharedKernel.Domain.ValueObjects;
using VShop.SharedKernel.EventSourcing.Aggregates;

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
        
        Task<IList<IdentifiedEvent<IBaseEvent>>> SaveAsync
        (
            TAggregate aggregate,
            Guid causationId,
            Guid correlationId,
            CancellationToken cancellationToken = default
        );

        Task PublishAsync
        (
            IEnumerable<IIdentifiedEvent<IBaseEvent>> events,
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