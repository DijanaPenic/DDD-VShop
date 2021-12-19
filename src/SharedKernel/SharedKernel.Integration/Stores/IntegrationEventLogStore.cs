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
using VShop.SharedKernel.Integration.Stores.Contracts;

namespace VShop.SharedKernel.Integration.Stores
{
    public class IntegrationEventLogStore : IIntegrationEventLogStore
    {
        private readonly IntegrationContext _integrationContext;
        
        public IntegrationEventLogStore(IntegrationContext integrationContext)
            => _integrationContext = integrationContext;

        public async Task<IEnumerable<IntegrationEventLog>> RetrieveEventsPendingPublishAsync
        (
            Guid transactionId,
            CancellationToken cancellationToken = default
        )
        {
            List<IntegrationEventLog> result = await _integrationContext.IntegrationEventLogs.AsQueryable()
                .Where(ie => ie.TransactionId == transactionId && ie.State == EventState.NotPublished)
                .ToListAsync(cancellationToken);

            return result.Any() ? result.OrderBy(ie => ie.DateCreated) : Enumerable.Empty<IntegrationEventLog>();
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
            IntegrationEventLog integrationEventLogEntry = await _integrationContext.IntegrationEventLogs
                .SingleAsync(ie => ie.EventId == eventId, cancellationToken);
            
            integrationEventLogEntry.State = status;
            if (status is EventState.InProgress) integrationEventLogEntry.TimesSent++;

            _integrationContext.IntegrationEventLogs.Update(integrationEventLogEntry);

            await _integrationContext.SaveChangesAsync(cancellationToken);
        }
    }
}