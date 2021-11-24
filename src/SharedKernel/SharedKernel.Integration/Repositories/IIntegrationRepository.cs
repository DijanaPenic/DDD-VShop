using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

using VShop.SharedKernel.Messaging.Events;

namespace VShop.SharedKernel.Integration.Repositories
{
    public interface IIntegrationRepository
    {
        Task SaveAsync(IIntegrationEvent @event, CancellationToken cancellationToken = default);
        Task<IEnumerable<IIntegrationEvent>> LoadAsync(CancellationToken cancellationToken = default);
    }
}