using System;
using System.Threading;
using System.Threading.Tasks;

using VShop.SharedKernel.Messaging.Events;

namespace VShop.SharedKernel.Integration.Services.Contracts
{
    public interface IIntegrationEventService
    {
        Task PublishEventsAsync(Guid transactionId, CancellationToken cancellationToken = default);
        Task SaveEventAsync(IIdentifiedEvent<IBaseEvent> @event, CancellationToken cancellationToken = default);
    }
}