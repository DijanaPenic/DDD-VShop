using System;
using System.Threading;
using System.Threading.Tasks;
using Serilog;
using EventStore.Client;
using Microsoft.Extensions.DependencyInjection;

using VShop.SharedKernel.Messaging;
using VShop.SharedKernel.Messaging.Events;
using VShop.SharedKernel.Messaging.Events.Publishing;
using VShop.SharedKernel.Messaging.Events.Publishing.Contracts;
using VShop.SharedKernel.EventStoreDb.Extensions;
using VShop.SharedKernel.EventStoreDb.Subscriptions;
using VShop.SharedKernel.EventStoreDb.Subscriptions.Infrastructure;

namespace VShop.SharedKernel.Integration.Projections
{
    public class IntegrationEventPublisher : ISubscriptionHandler
    {
        private readonly ILogger _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IEventBus _eventBus;

        public IntegrationEventPublisher
        (
            ILogger logger,
            IServiceProvider serviceProvider,
            IEventBus eventBus
        )
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _eventBus = eventBus;
        }

        public async Task ProjectAsync
        (
            ResolvedEvent resolvedEvent,
            Func<SubscriptionContext, Task> checkpointUpdate,
            CancellationToken cancellationToken = default
        )
        {
            IMessage message = resolvedEvent.DeserializeData<IMessage>();
            if (message is not IIntegrationEvent integrationEvent) return;
            
            _logger.Debug("Projecting integration event: {Message}", integrationEvent);
            
            using IServiceScope scope = _serviceProvider.CreateScope();
            SubscriptionContext subscriptionContext = scope.ServiceProvider.GetRequiredService<SubscriptionContext>();
            
            await checkpointUpdate(subscriptionContext);
            
            // TODO - need to figure out how to handle these exceptions. This will stop further integration projections.
            await _eventBus.Publish(integrationEvent, EventPublishStrategy.SyncStopOnException, cancellationToken);
        }
    }
}