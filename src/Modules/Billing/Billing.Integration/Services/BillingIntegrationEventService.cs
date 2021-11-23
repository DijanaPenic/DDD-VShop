using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Serilog;
using Newtonsoft.Json;

using VShop.SharedKernel.Messaging;
using VShop.SharedKernel.Messaging.Events;
using VShop.SharedKernel.Integration.Services;
using VShop.SharedKernel.Integration.Database.Entities;
using VShop.SharedKernel.EventSourcing.Repositories;
using VShop.Modules.Billing.Infrastructure;

using ILogger = Serilog.ILogger;

namespace VShop.Modules.Billing.Integration.Services
{
    public class BillingIntegrationEventService : IBillingIntegrationEventService
    {
        private readonly IIntegrationRepository _eventBus;
        private readonly BillingContext _billingContext;
        private readonly IIntegrationEventLogService _integrationEventLogService;

        private static readonly ILogger Logger = Log.ForContext<BillingIntegrationEventService>(); 

        public BillingIntegrationEventService
        (
            IIntegrationRepository eventBus,
            BillingContext billingContext,
            IIntegrationEventLogService integrationEventLogService
        )
        {
            _eventBus = eventBus;
            _billingContext = billingContext;
            _integrationEventLogService = integrationEventLogService;
        }

        public async Task PublishEventsAsync(Guid transactionId, CancellationToken cancellationToken = default)
        {
            IEnumerable<IntegrationEventLog> pendingEvents = await _integrationEventLogService.RetrieveEventLogsPendingPublishAsync(transactionId, cancellationToken);

            foreach (IntegrationEventLog pendingEvent in pendingEvents)
            {
                Logger.Information
                (
                    "Publishing integration event: {IntegrationEventId} from {AppName} - ({IntegrationEvent})",
                    pendingEvent.EventId, "Billing", pendingEvent.Content
                );

                try
                {
                    await _integrationEventLogService.MarkEventAsInProgressAsync(pendingEvent.EventId, cancellationToken);
                    
                    object target = JsonConvert.DeserializeObject(pendingEvent.Content, MessageTypeMapper.ToType(pendingEvent.EventTypeName));
                    await _eventBus.SaveAsync((IIntegrationEvent)target, cancellationToken);
                    
                    await _integrationEventLogService.MarkEventAsPublishedAsync(pendingEvent.EventId, cancellationToken);
                }
                catch (Exception ex)
                {
                    Logger.Error
                    (
                        ex,
                        "Error publishing integration event: {IntegrationEventId} from {AppName}",
                        pendingEvent.EventId, "Billing"
                    );

                    await _integrationEventLogService.MarkEventAsFailedAsync(pendingEvent.EventId, cancellationToken);
                }
            }
        }

        public async Task AddAndSaveEventAsync(IIntegrationEvent @event, CancellationToken cancellationToken = default)
        {
            Logger.Information
            (
                "Enqueuing integration event {IntegrationEventId} to repository ({IntegrationEvent})",
                @event.MessageId, @event
            );
            
            await _integrationEventLogService.SaveEventAsync(@event, _billingContext.GetCurrentTransaction(), cancellationToken);
        }
    }
}