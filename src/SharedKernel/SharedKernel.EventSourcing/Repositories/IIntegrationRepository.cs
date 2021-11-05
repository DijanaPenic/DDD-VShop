using System.Threading.Tasks;
using System.Collections.Generic;

using VShop.SharedKernel.Infrastructure.Messaging.Events;

namespace VShop.SharedKernel.EventSourcing.Repositories
{
    public interface IIntegrationRepository
    {
        Task SaveAsync(IIntegrationEvent @event);
        Task<IEnumerable<IIntegrationEvent>> LoadAsync();
    }
}