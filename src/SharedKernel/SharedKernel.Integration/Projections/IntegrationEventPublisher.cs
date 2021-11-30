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

using ILogger = Serilog.ILogger;

namespace VShop.SharedKernel.Integration.Projections
{
    public class IntegrationEventPublisher : ISubscriptionHandler
    {
        private readonly IEventBus _eventBus;
        
        // TODO - ambient context anti-pattern
        private static readonly ILogger Logger = Log.ForContext<IntegrationEventPublisher>();

        public IntegrationEventPublisher(IEventBus eventBus) => _eventBus = eventBus;

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
            
            Logger.Debug("Projecting integration event: {Message}", integrationEvent);
            
            // TODO - need to figure out how to handle these exceptions. This will stop further integration projections.
            return _eventBus.Publish(integrationEvent, EventPublishStrategy.SyncStopOnException, cancellationToken);
        }
    }
}