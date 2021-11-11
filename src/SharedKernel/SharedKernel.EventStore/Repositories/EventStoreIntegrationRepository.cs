using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using EventStore.Client;

using VShop.SharedKernel.EventStore.Extensions;
using VShop.SharedKernel.Infrastructure.Extensions;
using VShop.SharedKernel.Infrastructure.Messaging.Events;
using VShop.SharedKernel.EventSourcing.Repositories;

namespace VShop.SharedKernel.EventStore.Repositories
{
    public class EventStoreIntegrationRepository : IIntegrationRepository
    {
        private readonly EventStoreClient _eventStoreClient;

        public EventStoreIntegrationRepository(EventStoreClient eventStoreClient)
            => _eventStoreClient = eventStoreClient;

        public async Task SaveAsync(IIntegrationEvent @event, CancellationToken cancellationToken = default)
        {
            if (@event is null)
                throw new ArgumentNullException(nameof(@event));

            string streamName = GetStreamName();

            await _eventStoreClient.AppendToStreamWithRetryAsync
            (
                streamName,
                StreamState.Any,
                new[] { @event },
                cancellationToken
            );
        }

        public async Task<IEnumerable<IIntegrationEvent>> LoadAsync(CancellationToken cancellationToken = default)
        {
            string streamName = GetStreamName();
            
            IList<IIntegrationEvent> messages = await _eventStoreClient.ReadStreamForwardAsync<IIntegrationEvent>
            (
                streamName,
                StreamPosition.Start,
                cancellationToken
            );

            return messages;
        }
        
        private string GetStreamName()
            => $"{_eventStoreClient.ConnectionName}/integration".ToSnakeCase();
    }
}