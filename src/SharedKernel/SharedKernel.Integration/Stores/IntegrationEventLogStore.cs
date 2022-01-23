using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

using VShop.SharedKernel.Integration.DAL;
using VShop.SharedKernel.Integration.DAL.Entities;
using VShop.SharedKernel.Messaging.Events;
using VShop.SharedKernel.Integration.Stores.Contracts;

namespace VShop.SharedKernel.Integration.Stores
{
    public class IntegrationEventLogStore : IIntegrationEventLogStore
    {
        private readonly IntegrationDbContext _integrationDbContext;
        
        public IntegrationEventLogStore(IntegrationDbContext integrationDbContext)
            => _integrationDbContext = integrationDbContext;

        public async Task<IReadOnlyList<IntegrationEventLog>> RetrieveEventsPendingPublishAsync
        (
            Guid transactionId,
            CancellationToken cancellationToken = default
        )
            => await _integrationDbContext.IntegrationEventLogs
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

            await _integrationDbContext.Database.UseTransactionAsync(transaction.GetDbTransaction(), cancellationToken);
            await _integrationDbContext.IntegrationEventLogs.AddAsync(eventLogEntry, cancellationToken);

            await _integrationDbContext.SaveChangesAsync(cancellationToken);
        }

        public Task MarkEventAsPublishedAsync(Guid eventId, CancellationToken cancellationToken = default)
            => UpdateEventStatusAsync(eventId, EventState.Published, cancellationToken);

        public Task MarkEventAsInProgressAsync(Guid eventId, CancellationToken cancellationToken = default)
            => UpdateEventStatusAsync(eventId, EventState.InProgress, cancellationToken);

        public Task MarkEventAsFailedAsync(Guid eventId, CancellationToken cancellationToken = default)
            => UpdateEventStatusAsync(eventId, EventState.PublishFailed, cancellationToken);

        private async Task UpdateEventStatusAsync(Guid eventId, EventState status, CancellationToken cancellationToken = default)
        {
            IntegrationEventLog integrationEventLogEntry = await _integrationDbContext.IntegrationEventLogs
                .SingleAsync(ie => ie.Id == eventId, cancellationToken);
            
            integrationEventLogEntry.State = status;
            if (status is EventState.InProgress) integrationEventLogEntry.TimesSent++;

            _integrationDbContext.IntegrationEventLogs.Update(integrationEventLogEntry);

            await _integrationDbContext.SaveChangesAsync(cancellationToken);
        }
    }
}