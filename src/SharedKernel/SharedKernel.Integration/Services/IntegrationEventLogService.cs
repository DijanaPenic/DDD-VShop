using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

using VShop.SharedKernel.Messaging.Events;
using VShop.SharedKernel.Integration.Infrastructure;
using VShop.SharedKernel.Integration.Infrastructure.Entities;
using VShop.SharedKernel.Integration.Services.Contracts;

namespace VShop.SharedKernel.Integration.Services
{
    public class IntegrationEventLogService : IIntegrationEventLogService
    {
        private readonly IntegrationContext _integrationContext;

        // TODO - this should be repository class
        public IntegrationEventLogService(IntegrationContext integrationContext)
            => _integrationContext = integrationContext;

        public async Task<IEnumerable<IntegrationEventLog>> RetrieveEventLogsPendingPublishAsync
        (
            Guid transactionId,
            CancellationToken cancellationToken = default
        )
        {
            List<IntegrationEventLog> result = await _integrationContext.IntegrationEventLogs.AsQueryable()
                .Where(ie => ie.TransactionId == transactionId && ie.State == EventState.NotPublished)
                .ToListAsync(cancellationToken);

            return result.Any() ? result.OrderBy(ie => ie.DateCreatedUtc) : Enumerable.Empty<IntegrationEventLog>();
        }

        public async Task SaveEventAsync
        (
            IIntegrationEvent @event,
            IDbContextTransaction transaction,
            CancellationToken cancellationToken = default
        )
        {
            if (transaction is null) throw new ArgumentNullException(nameof(transaction));

            IntegrationEventLog eventLogEntry = new(@event, transaction.TransactionId);

            await _integrationContext.Database.UseTransactionAsync(transaction.GetDbTransaction(), cancellationToken);
            await _integrationContext.IntegrationEventLogs.AddAsync(eventLogEntry, cancellationToken);

            await _integrationContext.SaveChangesAsync(cancellationToken);
        }

        public Task MarkEventAsPublishedAsync(Guid eventId, CancellationToken cancellationToken = default)
            => UpdateEventStatusAsync(eventId, EventState.Published, cancellationToken);

        public Task MarkEventAsInProgressAsync(Guid eventId, CancellationToken cancellationToken = default)
            => UpdateEventStatusAsync(eventId, EventState.InProgress, cancellationToken);

        public Task MarkEventAsFailedAsync(Guid eventId, CancellationToken cancellationToken = default)
            => UpdateEventStatusAsync(eventId, EventState.PublishedFailed, cancellationToken);

        private async Task UpdateEventStatusAsync(Guid eventId, EventState status, CancellationToken cancellationToken = default)
        {
            IntegrationEventLog eventLogEntry = await _integrationContext.IntegrationEventLogs
                .SingleAsync(ie => ie.EventId == eventId, cancellationToken);
            eventLogEntry.State = status;

            if (status is EventState.InProgress) eventLogEntry.TimesSent++;

            _integrationContext.IntegrationEventLogs.Update(eventLogEntry);

            await _integrationContext.SaveChangesAsync(cancellationToken);
        }
    }
}