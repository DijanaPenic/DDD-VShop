﻿using Serilog;
using EventStore.Client;
using Microsoft.Extensions.DependencyInjection;
using VShop.SharedKernel.EventStoreDb.Extensions;
using VShop.SharedKernel.EventStoreDb.Subscriptions;
using VShop.SharedKernel.EventStoreDb.Subscriptions.DAL;
using VShop.SharedKernel.Infrastructure.Events;
using VShop.SharedKernel.Integration.Stores.Contracts;

namespace VShop.SharedKernel.Integration.Projections
{
    public class IntegrationEventProjectionToEventStore : ISubscriptionHandler
    {
        private readonly ILogger _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IIntegrationEventStore _integrationRepository;

        public IntegrationEventProjectionToEventStore
        (
            ILogger logger,
            IServiceProvider serviceProvider,
            IIntegrationEventStore integrationRepository
        )
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _integrationRepository = integrationRepository;
        }
        
        public async Task ProjectAsync
        (
            ResolvedEvent resolvedEvent,
            Func<SubscriptionDbContext, Task> checkpointUpdate,
            CancellationToken cancellationToken = default
        )
        {
            IIntegrationEvent integrationEvent = resolvedEvent.Deserialize<IIntegrationEvent>();
            if (integrationEvent is null) return;
            
            _logger.Debug("Projecting integration event: {Message}", integrationEvent);

            await _integrationRepository.SaveAsync(integrationEvent, cancellationToken);
            
            // Update the checkpoint after successful projection.
            using IServiceScope scope = _serviceProvider.CreateScope();
            SubscriptionDbContext subscriptionDbContext = scope.ServiceProvider.GetRequiredService<SubscriptionDbContext>();
            
            await checkpointUpdate(subscriptionDbContext);
        }
    }
}