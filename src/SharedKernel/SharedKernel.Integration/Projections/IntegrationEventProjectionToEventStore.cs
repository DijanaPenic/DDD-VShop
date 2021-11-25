using Serilog;
using System.Threading;
using System.Threading.Tasks;

using VShop.SharedKernel.Messaging;
using VShop.SharedKernel.Messaging.Events;
using VShop.SharedKernel.EventStoreDb.Messaging;
using VShop.SharedKernel.EventStoreDb.Subscriptions;
using VShop.SharedKernel.Integration.Repositories.Contracts;

using ILogger = Serilog.ILogger;

namespace VShop.SharedKernel.Integration.Projections
{
    public class IntegrationEventProjectionToEventStore : ISubscriptionHandler
    {
        private readonly IIntegrationRepository _integrationRepository;
        
        private static readonly ILogger Logger = Log.ForContext<IntegrationEventProjectionToEventStore>(); 
        
        public IntegrationEventProjectionToEventStore(IIntegrationRepository integrationRepository)
            => _integrationRepository = integrationRepository;

        public Task ProjectAsync(IMessage message, IMessageMetadata _, CancellationToken cancellationToken)
        {
            if (message is not IIntegrationEvent integrationEvent) return Task.CompletedTask;
            
            Logger.Debug("Projecting integration event: {Message}", integrationEvent);

            return _integrationRepository.SaveAsync(integrationEvent, cancellationToken);
        }
    }
}