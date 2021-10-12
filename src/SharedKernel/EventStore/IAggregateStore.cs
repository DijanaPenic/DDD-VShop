using System.Threading.Tasks;

using VShop.SharedKernel.EventSourcing;
using VShop.SharedKernel.Infrastructure.Domain;

namespace VShop.SharedKernel.EventStore
{
    public interface IAggregateStore<TA, TKey>
        where TKey : ValueObject
        where TA : AggregateRoot<TKey>
    {
        Task SaveAsync(TA aggregate);
        
        Task<bool> ExistsAsync(TKey aggregateId);
        
        Task<TA> LoadAsync(TKey aggregateId);
    }
}