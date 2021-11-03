using System;
using System.Threading.Tasks;

using VShop.SharedKernel.Domain.ValueObjects;

namespace VShop.SharedKernel.EventSourcing.Contracts
{
    public interface IAggregateRepository<TA, TKey>
        where TKey : ValueObject
        where TA : AggregateRoot<TKey>
    {
        Task SaveAsync(TA aggregate);
        
        Task<bool> ExistsAsync(TKey aggregateId);
        
        Task<TA> LoadAsync(TKey aggregateId, Guid? messageId = null, Guid? correlationId = null);
    }
}