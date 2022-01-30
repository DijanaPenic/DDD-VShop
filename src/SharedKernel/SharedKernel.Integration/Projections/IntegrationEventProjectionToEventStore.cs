using Serilog;
using EventStore.Client;
using Microsoft.Extensions.DependencyInjection;

using VShop.SharedKernel.Subscriptions;
using VShop.SharedKernel.Subscriptions.DAL;
using VShop.SharedKernel.EventStoreDb.Extensions;
using VShop.SharedKernel.Integration.Stores.Contracts;
using VShop.SharedKernel.Infrastructure.Events.Contracts;
using VShop.SharedKernel.Infrastructure.Messaging;
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
            MessageEnvelope<IIntegrationEvent> eventEnvelope = resolvedEvent.Deserialize<IIntegrationEvent>(_messageRegistry);
            if (eventEnvelope is null) return;
            
            _logger.Debug("Projecting integration event: {Message}", eventEnvelope.Message);

            await _integrationEventStore.SaveAsync(eventEnvelope, cancellationToken);
            
            // Update the checkpoint after successful projection.
            using IServiceScope scope = _serviceProvider.CreateScope();
            SubscriptionDbContext subscriptionDbContext = scope.ServiceProvider.GetRequiredService<SubscriptionDbContext>();
            
            await checkpointUpdate(subscriptionDbContext);
        }
    }
}