using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

using VShop.SharedKernel.Infrastructure.Contexts.Contracts;
using VShop.SharedKernel.Integration.DAL;
using VShop.SharedKernel.Integration.DAL.Entities;
using VShop.SharedKernel.Integration.Stores.Contracts;
using VShop.SharedKernel.PostgresDb.Contracts;
using VShop.SharedKernel.Infrastructure.Events.Contracts;
using VShop.SharedKernel.Infrastructure.Messaging;
using VShop.SharedKernel.Infrastructure.Messaging.Contracts;
using VShop.SharedKernel.Infrastructure.Types;

namespace VShop.SharedKernel.Integration.Stores
{
    public class IntegrationEventOutbox : IIntegrationEventOutbox
    {
        private readonly IntegrationDbContext _integrationDbContext;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMessageRegistry _messageRegistry;
        private readonly IContext _context;

        public IntegrationEventOutbox
        (
            IntegrationDbContext integrationDbContext,
            IUnitOfWork unitOfWork,
            IMessageRegistry messageRegistry,
            IContext context
        )
        {
            _integrationDbContext = integrationDbContext;
            _unitOfWork = unitOfWork;
            _messageRegistry = messageRegistry;
            _context = context;
        }

        public async Task<IReadOnlyList<IntegrationEventLog>> RetrieveEventsPendingPublishAsync
        (
            Guid transactionId,
            CancellationToken cancellationToken = default
        ) 
            => await _integrationDbContext.IntegrationEventLogs
                .Where(el => el.TransactionId == transactionId && el.State == EventState.NotPublished)
                .OrderBy(el => el.DateCreated)
                .ToListAsync(cancellationToken);

        public async Task SaveEventAsync(IIntegrationEvent @event, CancellationToken cancellationToken = default)
        {
            IDbContextTransaction transaction = _unitOfWork.CurrentTransaction;
            IntegrationEventLog eventLogEntry = new
            (
                @event,
                new MessageContext(SequentialGuid.Create(), _context),
                transaction.TransactionId,
                _messageRegistry
            );

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