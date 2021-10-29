using System.Threading.Tasks;

using VShop.SharedKernel.Domain;
using VShop.SharedKernel.EventSourcing;

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