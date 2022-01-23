using Serilog;
using VShop.SharedKernel.Infrastructure.Events;
using VShop.SharedKernel.PostgresDb;
using VShop.SharedKernel.Integration.DAL.Entities;
using VShop.SharedKernel.Integration.Stores.Contracts;
using VShop.SharedKernel.Integration.Services.Contracts;

namespace VShop.SharedKernel.Integration.Services
{
    public class IntegrationEventService : IIntegrationEventService
    {
        private readonly ILogger _logger;
        private readonly DbContextBase _dbContext;
        private readonly IIntegrationEventStore _integrationEventRepository;
        private readonly IIntegrationEventLogStore _integrationEventLogRepository;

        public IntegrationEventService
        (
            ILogger logger,
            MainDbContextProvider dbContextProvider,
            IIntegrationEventStore integrationEventRepository,
            IIntegrationEventLogStore integrationEventLogRepository
        )
        {
            _logger = logger;
            _dbContext = dbContextProvider();
            _integrationEventRepository = integrationEventRepository;
            _integrationEventLogRepository = integrationEventLogRepository;
        }

        public async Task PublishEventsAsync(Guid transactionId, CancellationToken cancellationToken = default)
        {
            IEnumerable<IntegrationEventLog> pendingEventLogs = await _integrationEventLogRepository
                .RetrieveEventsPendingPublishAsync(transactionId, cancellationToken);

            foreach (IntegrationEventLog pendingEventLog in pendingEventLogs)
            {
                _logger.Information
                (
                    "Publishing integration event: {IntegrationEventId} - ({IntegrationEvent})",
                    pendingEventLog.Id, pendingEventLog.Body
                );

                try
                {
                    await _integrationEventLogRepository.MarkEventAsInProgressAsync
                    (
                        pendingEventLog.Id,
                        cancellationToken
                    );

                    await _integrationEventRepository.SaveAsync
                    (
                        pendingEventLog.GetEvent(),
                        cancellationToken
                    );

                    await _integrationEventLogRepository.MarkEventAsPublishedAsync
                    (
                        pendingEventLog.Id,
                        cancellationToken
                    );
                }
                catch (Exception ex)
                {
                    _logger.Error
                    (
                        ex,
                        "Error publishing integration event: {IntegrationEventId}",
                        pendingEventLog.Id
                    );

                    await _integrationEventLogRepository.MarkEventAsFailedAsync(pendingEventLog.Id, cancellationToken);
                }
            }
        }

        public async Task SaveEventAsync(IIntegrationEvent @event, CancellationToken cancellationToken = default)
        {
            _logger.Information
            (
                "Enqueuing integration event {IntegrationEventId} - ({IntegrationEvent})",
                @event.Metadata.MessageId, @event
            );
            
            await _integrationEventLogRepository.SaveEventAsync(@event, _dbContext.CurrentTransaction, cancellationToken);
        }
    }
}