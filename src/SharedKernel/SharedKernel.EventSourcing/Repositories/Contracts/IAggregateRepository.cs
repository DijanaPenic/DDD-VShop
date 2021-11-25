using System;
using System.Threading;
using System.Threading.Tasks;

using VShop.SharedKernel.Domain.ValueObjects;
using VShop.SharedKernel.EventSourcing.Aggregates;

namespace VShop.SharedKernel.EventSourcing.Repositories.Contracts
{
    public interface IAggregateRepository<TA, TKey>
        where TKey : ValueObject
        where TA : AggregateRoot<TKey>
    {
        Task SaveAsync(TA aggregate, CancellationToken cancellationToken = default);
        Task<TA> LoadAsync(TKey aggregateId, Guid? causationId = null, Guid? correlationId = null, CancellationToken cancellationToken = default);
    }
}