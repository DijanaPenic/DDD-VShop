using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

using VShop.SharedKernel.Messaging.Events;
using VShop.SharedKernel.Integration.Database;
using VShop.SharedKernel.Integration.Database.Entities;

namespace VShop.SharedKernel.Integration.Services
{
    public class IntegrationEventLogService : IIntegrationEventLogService
    {
        private readonly IntegrationContext _integrationContext;

        public IntegrationEventLogService(IntegrationContext integrationContext)
            => _integrationContext = integrationContext;

        public async Task<IEnumerable<IntegrationEventLog>> RetrieveEventLogsPendingPublishAsync
        (
            Guid transactionId,
            CancellationToken cancellationToken = default
        )
        {
            List<IntegrationEventLog> result = await _integrationContext.IntegrationEventLogs
                .Where(ie => ie.TransactionId == transactionId && ie.State == EventState.NotPublished)
                .ToListAsync(cancellationToken);

            if (result is not null && result.Any()) return result.OrderBy(ie => ie.DateCreatedUtc);

            return Enumerable.Empty<IntegrationEventLog>();
        }

        public Task SaveEventAsync
        (
            IIntegrationEvent @event,
            IDbContextTransaction transaction,
            CancellationToken cancellationToken = default
        )
        {
            if (transaction is null) throw new ArgumentNullException(nameof(transaction));

            IntegrationEventLog eventLogEntry = new(@event, transaction.TransactionId);

            _integrationContext.Database.UseTransaction(transaction.GetDbTransaction());
            _integrationContext.IntegrationEventLogs.Add(eventLogEntry);

            return _integrationContext.SaveChangesAsync(cancellationToken);
        }

        public Task MarkEventAsPublishedAsync(Guid eventId, CancellationToken cancellationToken = default)
            => UpdateEventStatusAsync(eventId, EventState.Published, cancellationToken);

        public Task MarkEventAsInProgressAsync(Guid eventId, CancellationToken cancellationToken = default)
            => UpdateEventStatusAsync(eventId, EventState.InProgress, cancellationToken);

        public Task MarkEventAsFailedAsync(Guid eventId, CancellationToken cancellationToken = default)
            => UpdateEventStatusAsync(eventId, EventState.PublishedFailed, cancellationToken);

        private Task UpdateEventStatusAsync(Guid eventId, EventState status, CancellationToken cancellationToken = default)
        {
            IntegrationEventLog eventLogEntry = _integrationContext.IntegrationEventLogs
                .Single(ie => ie.EventId == eventId);
            eventLogEntry.State = status;

            if (status is EventState.InProgress) eventLogEntry.TimesSent++;

            _integrationContext.IntegrationEventLogs.Update(eventLogEntry);

            return _integrationContext.SaveChangesAsync(cancellationToken);
        }
    }
}