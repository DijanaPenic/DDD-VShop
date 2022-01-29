using Serilog;

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
        private readonly IMessageContextRegistry _messageContextRegistry;
        private readonly IIntegrationEventStore _integrationEventStore;
        private readonly IIntegrationEventOutbox _integrationEventOutbox;

        public IntegrationEventService
        (
            ILogger logger,
            IMessageRegistry messageRegistry,
            IMessageContextRegistry messageContextRegistry,
            IIntegrationEventStore integrationEventStore,
            IIntegrationEventOutbox integrationEventOutbox
        )
        {
            _logger = logger;
            _messageRegistry = messageRegistry;
            _messageContextRegistry = messageContextRegistry;
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
                    "Publishing integration event {IntegrationEventType}: {IntegrationEventId}",
                    pendingEventLog.TypeName, pendingEventLog.Id
                );

                try
                {
                    await _integrationEventOutbox.MarkEventAsInProgressAsync(pendingEventLog.Id, cancellationToken);
                    
                    (IIntegrationEvent integrationEvent, IMessageContext messageContext) = pendingEventLog
                        .GetEvent(_messageRegistry);
                    
                    _messageContextRegistry.Set(integrationEvent, messageContext);
                    
                    await _integrationEventStore.SaveAsync(integrationEvent, cancellationToken);
                    await _integrationEventOutbox.MarkEventAsPublishedAsync(pendingEventLog.Id, cancellationToken);
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
                "Enqueuing integration event {IntegrationEventType} - ({IntegrationEvent})",
                @event.GetType().Name, @event
            );
            
            await _integrationEventOutbox.SaveEventAsync(@event, cancellationToken);
        }
    }
}