using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using EventStore.Client;

using VShop.SharedKernel.Messaging.Events;
using VShop.SharedKernel.EventStoreDb.Extensions;
using VShop.SharedKernel.Infrastructure.Extensions;
using VShop.SharedKernel.Infrastructure.Services.Contracts;
using VShop.SharedKernel.Integration.Stores.Contracts;

namespace VShop.SharedKernel.Integration.Stores
{
    public class IntegrationEventStore : IIntegrationEventStore
    {
        private readonly IClockService _clockService;
        private readonly EventStoreClient _eventStoreClient;

        public IntegrationEventStore(IClockService clockService, EventStoreClient eventStoreClient)
        {
            _clockService = clockService;
            _eventStoreClient = eventStoreClient;
        }

        public async Task SaveAsync(IIntegrationEvent @event, CancellationToken cancellationToken = default)
        {
            if (@event is null)
                throw new ArgumentNullException(nameof(@event));

            await _eventStoreClient.AppendToStreamAsync
            (
                GetStreamName(),
                StreamState.Any,
                new[] { @event },
                _clockService.Now,
                cancellationToken
            );
        }

        public Task<IReadOnlyList<IIntegrationEvent>> LoadAsync(CancellationToken cancellationToken = default)
            => _eventStoreClient.ReadStreamForwardAsync<IIntegrationEvent>
            (
                GetStreamName(),
                cancellationToken
            );
        
        private string GetStreamName()
            => $"{_eventStoreClient.ConnectionName}/integration".ToSnakeCase();
    }
}