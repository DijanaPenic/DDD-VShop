using System.Threading.Tasks;

using VShop.SharedKernel.EventSourcing;
using VShop.SharedKernel.Domain.ValueObjects;

namespace VShop.SharedKernel.EventStore.Repositories.Contracts
{
    public interface IEventStoreAggregateRepository<TA, TKey>
        where TKey : ValueObject
        where TA : AggregateRoot<TKey>
    {
        Task SaveAsync(TA aggregate);
        
        Task<bool> ExistsAsync(TKey aggregateId);
        
        Task<TA> LoadAsync(TKey aggregateId);
    }
}