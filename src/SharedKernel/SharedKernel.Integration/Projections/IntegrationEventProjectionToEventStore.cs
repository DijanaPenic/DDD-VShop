using System;
using Serilog;
using System.Threading;
using System.Threading.Tasks;
using EventStore.Client;
using Microsoft.Extensions.DependencyInjection;
using VShop.SharedKernel.EventStoreDb.Extensions;

using VShop.SharedKernel.Messaging;
using VShop.SharedKernel.Messaging.Events;
using VShop.SharedKernel.EventStoreDb.Subscriptions;
using VShop.SharedKernel.EventStoreDb.Subscriptions.Infrastructure;
using VShop.SharedKernel.Integration.Repositories.Contracts;

namespace VShop.SharedKernel.Integration.Projections
{
    public class IntegrationEventProjectionToEventStore : ISubscriptionHandler
    {
        private readonly ILogger _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IIntegrationEventRepository _integrationRepository;

        public IntegrationEventProjectionToEventStore
        (
            ILogger logger,
            IServiceProvider serviceProvider,
            IIntegrationEventRepository integrationRepository
        )
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _integrationRepository = integrationRepository;
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
            
            await _integrationRepository.SaveAsync(integrationEvent, cancellationToken);
        }
    }
}