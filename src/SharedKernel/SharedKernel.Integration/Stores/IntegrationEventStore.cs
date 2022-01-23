using EventStore.Client;
using VShop.SharedKernel.EventStoreDb.Extensions;
using VShop.SharedKernel.Infrastructure.Events;
using VShop.SharedKernel.Integration.Stores.Contracts;

namespace VShop.SharedKernel.Integration.Stores
{
    public class IntegrationEventStore : IIntegrationEventStore
    {
        private readonly EventStoreClient _eventStoreClient;

        public IntegrationEventStore(EventStoreClient eventStoreClient) => _eventStoreClient = eventStoreClient;

        public async Task SaveAsync(IIntegrationEvent @event, CancellationToken cancellationToken = default)
        {
            if (@event is null)
                throw new ArgumentNullException(nameof(@event));

            await _eventStoreClient.AppendToStreamAsync
            (
                GetStreamName(),
                StreamState.Any,
                new[] { @event },
                cancellationToken
            );
        }
        
        public async Task<IReadOnlyList<IIntegrationEvent>> LoadAsync(CancellationToken cancellationToken = default)
            => await _eventStoreClient.ReadStreamForwardAsync<IIntegrationEvent>
            (
                GetStreamName(),
                cancellationToken
            );

        public static string GetStreamName() => "integration";
    }
}