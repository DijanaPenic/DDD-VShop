using System.Threading.Tasks;

using VShop.SharedKernel.Infrastructure.Messaging;

namespace VShop.SharedKernel.EventStore.Repositories.Contracts
{
    public interface IEventStoreIntegrationRepository
    {
        Task SaveAsync(IIntegrationEvent @event);
        Task<IIntegrationEvent[]> LoadAsync();
    }
}