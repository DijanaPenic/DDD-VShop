using Serilog;
using System.Threading;
using System.Threading.Tasks;

using VShop.SharedKernel.Messaging;
using VShop.SharedKernel.Messaging.Events;
using VShop.SharedKernel.EventSourcing.Messaging;
using VShop.SharedKernel.EventSourcing.Projections;
using VShop.SharedKernel.EventSourcing.Repositories;

using ILogger = Serilog.ILogger;

namespace VShop.SharedKernel.EventStore.Projections
{
    public class IntegrationEventProjectionToEventStore : ISubscription
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