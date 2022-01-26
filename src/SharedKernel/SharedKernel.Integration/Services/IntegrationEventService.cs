﻿using Serilog;

using VShop.SharedKernel.Integration.DAL.Entities;
using VShop.SharedKernel.Integration.Stores.Contracts;
using VShop.SharedKernel.Integration.Services.Contracts;
using VShop.SharedKernel.Infrastructure.Events.Contracts;
using VShop.SharedKernel.Infrastructure.Messaging.Contracts;

namespace VShop.SharedKernel.Integration.Services
{
    public class IntegrationEventService : IIntegrationEventService
    {
        private readonly ILogger _logger;
        private readonly IMessageRegistry _messageRegistry;
        private readonly IIntegrationEventStore _integrationEventStore;
        private readonly IIntegrationEventOutbox _integrationEventOutbox;

        public IntegrationEventService
        (
            ILogger logger,
            IMessageRegistry messageRegistry,
            IIntegrationEventStore integrationEventStore,
            IIntegrationEventOutbox integrationEventOutbox
        )
        {
            _logger = logger;
            _messageRegistry = messageRegistry;
            _integrationEventStore = integrationEventStore;
            _integrationEventOutbox = integrationEventOutbox;
        }

        public async Task PublishEventsAsync(Guid transactionId, CancellationToken cancellationToken = default)
        {
            IEnumerable<IntegrationEventLog> pendingEventLogs = await _integrationEventOutbox
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
                    await _integrationEventOutbox.MarkEventAsInProgressAsync
                    (
                        pendingEventLog.Id,
                        cancellationToken
                    );

                    await _integrationEventStore.SaveAsync
                    (
                        pendingEventLog.GetEvent(_messageRegistry),
                        cancellationToken
                    );

                    await _integrationEventOutbox.MarkEventAsPublishedAsync
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

                    await _integrationEventOutbox.MarkEventAsFailedAsync(pendingEventLog.Id, cancellationToken);
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
            
            await _integrationEventOutbox.SaveEventAsync(@event, cancellationToken);
        }
    }
}