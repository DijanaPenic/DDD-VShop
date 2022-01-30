using EventStore.Client;

using VShop.SharedKernel.EventStoreDb;
using VShop.SharedKernel.Integration.Stores.Contracts;
using VShop.SharedKernel.Infrastructure.Events.Contracts;
using VShop.SharedKernel.Infrastructure.Messaging;
using VShop.SharedKernel.Infrastructure.Messaging.Contracts;

namespace VShop.SharedKernel.Integration.Stores
{
    public class IntegrationEventStore : IIntegrationEventStore
    {
        private readonly CustomEventStoreClient _eventStoreClient;
        private readonly IMessageContextRegistry _messageContextRegistry;

        public IntegrationEventStore
        (
            CustomEventStoreClient eventStoreClient, 
            IMessageContextRegistry messageContextRegistry
        )
        {
            _eventStoreClient = eventStoreClient;
            _messageContextRegistry = messageContextRegistry;
        }

        public async Task SaveAsync(MessageEnvelope<IIntegrationEvent> eventEnvelope, CancellationToken cancellationToken = default)
        {
            if (eventEnvelope is null)
                throw new ArgumentNullException(nameof(eventEnvelope));
            
            _messageContextRegistry.Set(eventEnvelope.Message, eventEnvelope.MessageContext);
            
            await _eventStoreClient.AppendToStreamAsync
            (
                GetStreamName(),
                StreamState.Any,
                new[] { eventEnvelope.Message },
                cancellationToken
            );
        }
        
        public Task<IReadOnlyList<MessageEnvelope<IIntegrationEvent>>> LoadAsync(CancellationToken cancellationToken = default)
            => _eventStoreClient.ReadStreamForwardAsync<IIntegrationEvent>
            (
                GetStreamName(),
                cancellationToken
            );

        public static string GetStreamName() => "integration";
    }
}