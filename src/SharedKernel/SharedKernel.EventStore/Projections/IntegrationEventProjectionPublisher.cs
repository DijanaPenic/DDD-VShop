using Serilog;
using System.Threading.Tasks;

using VShop.SharedKernel.EventSourcing;
using VShop.SharedKernel.Infrastructure.Messaging;

using ILogger = Serilog.ILogger;

namespace VShop.SharedKernel.EventStore.Projections
{
    public class IntegrationEventProjectionPublisher : ISubscription
    {
        private static readonly ILogger Logger = Log.ForContext<IntegrationEventProjectionPublisher>(); 
        private readonly Publisher _publisher;
        
        public IntegrationEventProjectionPublisher(Publisher publisher)
        {
            _publisher = publisher;
        }

        public Task ProjectAsync(IMessage message, MessageMetadata metadata)
        {
            if (message is not IIntegrationEvent integrationEvent) return Task.CompletedTask;
            
            Logger.Debug("Projecting integration event: {Message}", integrationEvent);
                
            return _publisher.Publish(integrationEvent, PublishStrategy.SyncStopOnException);
        }
    }
}