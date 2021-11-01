using Serilog;
using System.Threading.Tasks;

using VShop.SharedKernel.EventSourcing;
using VShop.SharedKernel.Infrastructure.Messaging;
using VShop.SharedKernel.EventStore.Repositories.Contracts;

using ILogger = Serilog.ILogger;

namespace VShop.SharedKernel.EventStore.Subscriptions
{
    public class EventStoreDbIntegrationProjection : ISubscription
    {
        private readonly IEventStoreIntegrationRepository _integrationRepository;

        private static readonly ILogger Logger = Log.ForContext<EventStoreDbIntegrationProjection>(); 
        
        public EventStoreDbIntegrationProjection(IEventStoreIntegrationRepository integrationRepository)
        {
            _integrationRepository = integrationRepository;
        }

        public Task ProjectAsync(IMessage message, MessageMetadata metadata)
        {
            if (message is not IIntegrationEvent integrationEvent) return Task.CompletedTask;
            
            Logger.Debug("Projecting integration event: {Message}", integrationEvent);
                
            return _integrationRepository.SaveAsync(integrationEvent);
        }
    }
}