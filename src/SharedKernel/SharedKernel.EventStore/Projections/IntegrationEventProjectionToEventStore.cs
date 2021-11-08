using Serilog;
using System.Threading.Tasks;

using VShop.SharedKernel.EventSourcing.Messaging;
using VShop.SharedKernel.EventSourcing.Projections;
using VShop.SharedKernel.EventSourcing.Repositories;
using VShop.SharedKernel.Infrastructure.Messaging;
using VShop.SharedKernel.Infrastructure.Messaging.Events;

using ILogger = Serilog.ILogger;

namespace VShop.SharedKernel.EventStore.Projections
{
    public class IntegrationEventProjectionToEventStore : ISubscription
    {
        private readonly IIntegrationRepository _integrationRepository;
        private static readonly ILogger Logger = Log.ForContext<IntegrationEventProjectionToEventStore>(); 
        
        public IntegrationEventProjectionToEventStore(IIntegrationRepository integrationRepository)
        {
            _integrationRepository = integrationRepository;
        }

        public Task ProjectAsync(IMessage message, IMessageMetadata _)
        {
            if (message is not IIntegrationEvent integrationEvent) return Task.CompletedTask;
            
            Logger.Debug("Projecting integration event: {Message}", integrationEvent);

            return _integrationRepository.SaveAsync(integrationEvent);
        }
    }
}