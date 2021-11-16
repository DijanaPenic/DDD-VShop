﻿using Serilog;
using System.Threading;
using System.Threading.Tasks;

using VShop.SharedKernel.EventSourcing.Messaging;
using VShop.SharedKernel.EventSourcing.Projections;
using VShop.SharedKernel.Infrastructure.Messaging;
using VShop.SharedKernel.Infrastructure.Messaging.Events;
using VShop.SharedKernel.Infrastructure.Messaging.Events.Publishing;

using ILogger = Serilog.ILogger;

namespace VShop.SharedKernel.EventStore.Projections
{
    public class IntegrationEventProjectionPublisher : ISubscription
    {
        private readonly IEventBus _eventBus;
        
        private static readonly ILogger Logger = Log.ForContext<IntegrationEventProjectionPublisher>();

        public IntegrationEventProjectionPublisher(IEventBus eventBus)
            => _eventBus = eventBus;

        public Task ProjectAsync(IMessage message, IMessageMetadata _, CancellationToken cancellationToken)
        {
            if (message is not IIntegrationEvent integrationEvent) return Task.CompletedTask;
            
            Logger.Debug("Projecting integration event: {Message}", integrationEvent);
            
            // TODO - need to figure out how to handle these exceptions. This will stop further integration projections.
            return _eventBus.Publish(integrationEvent, EventPublishStrategy.SyncStopOnException, cancellationToken);
        }
    }
}