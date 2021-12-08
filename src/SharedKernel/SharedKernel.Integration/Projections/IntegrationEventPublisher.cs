using Serilog;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;

using VShop.SharedKernel.Messaging;
using VShop.SharedKernel.Messaging.Events;
using VShop.SharedKernel.Messaging.Events.Publishing;
using VShop.SharedKernel.Messaging.Events.Publishing.Contracts;
using VShop.SharedKernel.EventStoreDb.Messaging;
using VShop.SharedKernel.EventStoreDb.Subscriptions;

namespace VShop.SharedKernel.Integration.Projections
{
    public class IntegrationEventPublisher : ISubscriptionHandler
    {
        private readonly ILogger _logger;
        private readonly IEventBus _eventBus;

        public IntegrationEventPublisher(ILogger logger, IEventBus eventBus)
        {
            _logger = logger;
            _eventBus = eventBus;
        }

        public Task ProjectAsync
        (
            IMessage message,
            IMessageMetadata metadata,
            IServiceScope scope,
            IDbContextTransaction transaction,
            CancellationToken cancellationToken = default
        )
        {
            if (message is not IIntegrationEvent integrationEvent) return Task.CompletedTask;
            
            _logger.Debug("Projecting integration event: {Message}", integrationEvent);
            
            // TODO - need to figure out how to handle these exceptions. This will stop further integration projections.
            return _eventBus.Publish(integrationEvent, EventPublishStrategy.SyncStopOnException, cancellationToken);
        }
    }
}