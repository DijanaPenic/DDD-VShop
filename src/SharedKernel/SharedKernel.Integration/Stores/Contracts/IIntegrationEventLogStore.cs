using Microsoft.EntityFrameworkCore.Storage;

using VShop.SharedKernel.Integration.DAL.Entities;
using VShop.SharedKernel.Infrastructure.Events.Contracts;

namespace VShop.SharedKernel.Integration.Stores.Contracts
{
    public interface IIntegrationEventLogStore
    {
        Task<IReadOnlyList<IntegrationEventLog>> RetrieveEventsPendingPublishAsync(Guid transactionId, CancellationToken cancellationToken = default);
        Task SaveEventAsync(IIntegrationEvent @event, IDbContextTransaction transaction, CancellationToken cancellationToken = default);
        Task MarkEventAsPublishedAsync(Guid eventId, CancellationToken cancellationToken = default);
        Task MarkEventAsInProgressAsync(Guid eventId, CancellationToken cancellationToken = default);
        Task MarkEventAsFailedAsync(Guid eventId, CancellationToken cancellationToken = default);
    }
}