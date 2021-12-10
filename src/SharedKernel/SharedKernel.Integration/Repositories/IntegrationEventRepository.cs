using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using EventStore.Client;

using VShop.SharedKernel.Messaging.Events;
using VShop.SharedKernel.EventStoreDb.Extensions;
using VShop.SharedKernel.Infrastructure.Extensions;
using VShop.SharedKernel.Infrastructure.Services.Contracts;
using VShop.SharedKernel.Integration.Repositories.Contracts;

namespace VShop.SharedKernel.Integration.Repositories
{
    public class IntegrationEventRepository : IIntegrationEventRepository
    {
        private readonly IClockService _clockService;
        private readonly EventStoreClient _eventStoreClient;

        public IntegrationEventRepository(IClockService clockService, EventStoreClient eventStoreClient)
        {
            _clockService = clockService;
            _eventStoreClient = eventStoreClient;
        }

        public async Task SaveAsync(IIntegrationEvent @event, CancellationToken cancellationToken = default)
        {
            if (@event is null)
                throw new ArgumentNullException(nameof(@event));

            string streamName = GetStreamName();

            await _eventStoreClient.AppendToStreamAsync
            (
                _clockService,
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