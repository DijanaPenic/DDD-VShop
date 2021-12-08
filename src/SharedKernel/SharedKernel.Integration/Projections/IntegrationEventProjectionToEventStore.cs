using Serilog;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;

using VShop.SharedKernel.Messaging;
using VShop.SharedKernel.Messaging.Events;
using VShop.SharedKernel.EventStoreDb.Messaging;
using VShop.SharedKernel.EventStoreDb.Subscriptions;
using VShop.SharedKernel.Integration.Repositories.Contracts;

namespace VShop.SharedKernel.Integration.Projections
{
    public class IntegrationEventProjectionToEventStore : ISubscriptionHandler
    {
        private readonly ILogger _logger;
        private readonly IIntegrationEventRepository _integrationRepository;

        public IntegrationEventProjectionToEventStore(ILogger logger, IIntegrationEventRepository integrationRepository)
        {
            _logger = logger;
            _integrationRepository = integrationRepository;
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

            return _integrationRepository.SaveAsync(integrationEvent, cancellationToken);
        }
    }
}