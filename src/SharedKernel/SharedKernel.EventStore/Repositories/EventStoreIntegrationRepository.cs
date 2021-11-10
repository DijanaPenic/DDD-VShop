using System;
using System.Linq;
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

            string streamName = GetIntegrationStreamName();
            
            await _eventStoreClient.AppendToStreamWithRetryAsync
            (
                streamName,
                StreamState.Any,
                cancellationToken,
                @event
            );
        }

        public Task<IEnumerable<IIntegrationEvent>> LoadAsync(CancellationToken cancellationToken = default)
        {
            string streamName = GetIntegrationStreamName();
            
            return _eventStoreClient.ReadStreamForwardAsync<IIntegrationEvent>
            (
                streamName,
                StreamPosition.Start,
                cancellationToken
            );
        }
        
        private string GetIntegrationStreamName()
            => $"{_eventStoreClient.ConnectionName}/integration".ToSnakeCase();
    }
}