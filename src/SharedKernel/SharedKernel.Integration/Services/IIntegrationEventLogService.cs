using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Storage;

using VShop.SharedKernel.Messaging.Events;
using VShop.SharedKernel.Integration.Database.Entities;

namespace VShop.SharedKernel.Integration.Services
{
    public interface IIntegrationEventLogService
    {
        Task<IEnumerable<IntegrationEventLog>> RetrieveEventLogsPendingPublishAsync(Guid transactionId, CancellationToken cancellationToken = default);
        Task SaveEventAsync(IIntegrationEvent @event, IDbContextTransaction transaction, CancellationToken cancellationToken = default);
        Task MarkEventAsPublishedAsync(Guid eventId, CancellationToken cancellationToken = default);
        Task MarkEventAsInProgressAsync(Guid eventId, CancellationToken cancellationToken = default);
        Task MarkEventAsFailedAsync(Guid eventId, CancellationToken cancellationToken = default);
    }
}