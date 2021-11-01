using System.Threading.Tasks;
using System.Collections.Generic;

using VShop.SharedKernel.Infrastructure.Messaging;

namespace VShop.SharedKernel.EventStore.Repositories.Contracts
{
    public interface IEventStoreIntegrationRepository
    {
        Task SaveAsync(IIntegrationEvent @event);
        Task<IEnumerable<IIntegrationEvent>> LoadAsync();
    }
}