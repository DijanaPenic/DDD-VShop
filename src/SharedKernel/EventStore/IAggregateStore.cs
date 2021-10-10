using System.Threading.Tasks;

using VShop.SharedKernel.EventSourcing;
using VShop.SharedKernel.Infrastructure.Domain;

namespace VShop.SharedKernel.EventStore
{
    public interface IAggregateStore
    {
        Task SaveAsync<T>(T aggregate)
            where T : AggregateRoot;

        Task<bool> ExistsAsync<T>(EntityId aggregateId)
            where T : AggregateRoot;

        Task<T> LoadAsync<T>(EntityId aggregateId)
            where T : AggregateRoot;
    }
}