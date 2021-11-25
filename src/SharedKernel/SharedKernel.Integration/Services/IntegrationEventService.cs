using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Serilog;
using Newtonsoft.Json;

using VShop.SharedKernel.Messaging;
using VShop.SharedKernel.Messaging.Events;
using VShop.SharedKernel.PostgresDb;
using VShop.SharedKernel.Integration.Services.Contracts;
using VShop.SharedKernel.Integration.Repositories.Contracts;
using VShop.SharedKernel.Integration.Infrastructure.Entities;

using ILogger = Serilog.ILogger;

namespace VShop.SharedKernel.Integration.Services
{
    public class IntegrationEventService<TDbContext> : IIntegrationEventService
        where TDbContext :  DbContextBase
    {
        private readonly IIntegrationRepository _eventBus;
        private readonly TDbContext _dbContext;
        private readonly IIntegrationEventLogService _integrationEventLogService;

        private static readonly ILogger Logger = Log.ForContext<IntegrationEventService<TDbContext>>(); 

        public IntegrationEventService
        (
            IIntegrationRepository eventBus,
            TDbContext dbContext,
            IIntegrationEventLogService integrationEventLogService
        )
        {
            _eventBus = eventBus;
            _dbContext = dbContext;
            _integrationEventLogService = integrationEventLogService;
        }

        public async Task PublishEventsAsync(Guid transactionId, CancellationToken cancellationToken = default)
        {
            IEnumerable<IntegrationEventLog> pendingEvents = await _integrationEventLogService.RetrieveEventLogsPendingPublishAsync(transactionId, cancellationToken);

            foreach (IntegrationEventLog pendingEvent in pendingEvents)
            {
                Logger.Information
                (
                    "Publishing integration event: {IntegrationEventId} - ({IntegrationEvent})",
                    pendingEvent.EventId, pendingEvent.Content
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
                        "Error publishing integration event: {IntegrationEventId}",
                        pendingEvent.EventId
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
            
            await _integrationEventLogService.SaveEventAsync(@event, _dbContext.GetCurrentTransaction(), cancellationToken);
        }
    }
}