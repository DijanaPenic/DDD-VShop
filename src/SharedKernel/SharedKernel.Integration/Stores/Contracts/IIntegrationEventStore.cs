using System.Threading;
using System.Threading.Tasks;

using VShop.SharedKernel.Messaging.Events;

namespace VShop.SharedKernel.Integration.Stores.Contracts
{
    public interface IIntegrationEventStore
    {
        Task SaveAsync(IIdentifiedEvent<IIntegrationEvent> @event, CancellationToken cancellationToken = default);
    }
}