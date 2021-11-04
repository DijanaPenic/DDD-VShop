using System.Threading.Tasks;
using System.Collections.Generic;

using VShop.SharedKernel.Infrastructure.Messaging;

namespace VShop.SharedKernel.EventSourcing.Repositories.Contracts
{
    public interface IIntegrationRepository
    {
        Task SaveAsync(IIntegrationEvent @event);
        Task<IEnumerable<IIntegrationEvent>> LoadAsync();
    }
}