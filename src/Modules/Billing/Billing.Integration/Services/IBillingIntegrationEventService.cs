using System;
using System.Threading;
using System.Threading.Tasks;

using VShop.SharedKernel.Messaging.Events;

namespace VShop.Modules.Billing.Integration.Services
{
    public interface IBillingIntegrationEventService
    {
        Task PublishEventsAsync(Guid transactionId, CancellationToken cancellationToken = default);
        Task AddAndSaveEventAsync(IIntegrationEvent @event, CancellationToken cancellationToken = default);
    }
}