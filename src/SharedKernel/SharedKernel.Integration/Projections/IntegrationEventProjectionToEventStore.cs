using Serilog;
using EventStore.Client;

using Microsoft.Extensions.DependencyInjection;
using VShop.SharedKernel.EventStoreDb.Extensions;
using VShop.SharedKernel.EventStoreDb.Subscriptions;
using VShop.SharedKernel.EventStoreDb.Subscriptions.DAL;
using VShop.SharedKernel.Integration.Stores.Contracts;
using VShop.SharedKernel.Infrastructure.Events.Contracts;
using VShop.SharedKernel.Infrastructure.Messaging.Contracts;

namespace VShop.SharedKernel.Integration.Projections
{
    public class IntegrationEventProjectionToEventStore : ISubscriptionHandler
    {
        private readonly ILogger _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IMessageRegistry _messageRegistry;
        private readonly IIntegrationEventStore _integrationEventStore;

        public IntegrationEventProjectionToEventStore
        (
            ILogger logger,
            IServiceProvider serviceProvider,
            IMessageRegistry messageRegistry,
            IIntegrationEventStore integrationEventStore
        )
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _messageRegistry = messageRegistry;
            _integrationEventStore = integrationEventStore;
        }
        
        public async Task ProjectAsync
        (
            ResolvedEvent resolvedEvent,
            Func<SubscriptionDbContext, Task> checkpointUpdate,
            CancellationToken cancellationToken = default
        )
        {
            IIntegrationEvent integrationEvent = resolvedEvent.Deserialize<IIntegrationEvent>(_messageRegistry);
            if (integrationEvent is null) return;
            
            _logger.Debug("Projecting integration event: {Message}", integrationEvent);

            await _integrationEventStore.SaveAsync(integrationEvent, cancellationToken);
            
            // Update the checkpoint after successful projection.
            using IServiceScope scope = _serviceProvider.CreateScope();
            SubscriptionDbContext subscriptionDbContext = scope.ServiceProvider.GetRequiredService<SubscriptionDbContext>();
            
            await checkpointUpdate(subscriptionDbContext);
        }
    }
}