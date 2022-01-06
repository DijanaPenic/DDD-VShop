using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Serilog;

using VShop.SharedKernel.PostgresDb;
using VShop.SharedKernel.Messaging.Events;
using VShop.SharedKernel.Integration.Stores.Contracts;
using VShop.SharedKernel.Integration.Services.Contracts;
using VShop.SharedKernel.Integration.Infrastructure.Entities;

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
                    pendingEventLog.EventId, pendingEventLog.Content
                );

                try
                {
                    await _integrationEventLogRepository.MarkEventAsInProgressAsync
                    (
                        pendingEventLog.EventId,
                        cancellationToken
                    );

                    await _integrationEventRepository.SaveAsync
                    (
                        pendingEventLog.GetEvent<IIntegrationEvent>(),
                        cancellationToken
                    );

                    await _integrationEventLogRepository.MarkEventAsPublishedAsync
                    (
                        pendingEventLog.EventId,
                        cancellationToken
                    );
                }
                catch (Exception ex)
                {
                    _logger.Error
                    (
                        ex,
                        "Error publishing integration event: {IntegrationEventId}",
                        pendingEventLog.EventId
                    );

                    await _integrationEventLogRepository.MarkEventAsFailedAsync(pendingEventLog.EventId, cancellationToken);
                }
            }
        }

        public async Task SaveEventAsync(IIntegrationEvent @event, CancellationToken cancellationToken = default)
        {
            _logger.Information
            (
                "Enqueuing integration event {IntegrationEventId} - ({IntegrationEvent})",
                @event.MessageId, @event
            );
            
            await _integrationEventLogRepository.SaveEventAsync(@event, _dbContext.CurrentTransaction, cancellationToken);
        }
    }
}