using System;
using System.Threading.Tasks;

using VShop.SharedKernel.EventSourcing;

namespace VShop.SharedKernel.EventStore
{
    public interface IAggregateStore
    {
        Task UpdateAsync<T>(long version, AggregateState<T>.Result update)
            where T : class, IAggregateState<T>, new();
        
        Task CreateAsync<T>(AggregateState<T>.Result create)
            where T : class, IAggregateState<T>, new();

        Task<T> LoadAsync<T>(Guid id)
            where T : IAggregateState<T>, new();
    }
}