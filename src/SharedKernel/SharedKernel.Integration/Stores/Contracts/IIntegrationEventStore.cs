using System.Threading;
using System.Threading.Tasks;

using VShop.SharedKernel.Messaging.Events;

namespace VShop.SharedKernel.Integration.Stores.Contracts
{
    public interface IIntegrationEventStore
    {
        Task SaveAsync(IIntegrationEvent @event, CancellationToken cancellationToken = default);
    }
}