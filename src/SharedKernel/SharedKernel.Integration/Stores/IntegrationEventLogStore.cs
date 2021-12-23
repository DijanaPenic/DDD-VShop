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

        public async Task<IReadOnlyList<IntegrationEventLog>> RetrieveEventsPendingPublishAsync
        (
            Guid transactionId,
            CancellationToken cancellationToken = default
        )
            => await _integrationContext.IntegrationEventLogs
                .Where(el => el.TransactionId == transactionId && el.State == EventState.NotPublished)
                .OrderBy(el => el.DateCreated)
                .ToListAsync(cancellationToken);

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
            => UpdateEventStatusAsync(eventId, EventState.PublishFailed, cancellationToken);

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