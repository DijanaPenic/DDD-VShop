using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using EventStore.ClientAPI;

using VShop.SharedKernel.EventStore.Helpers;
using VShop.SharedKernel.EventStore.Extensions;
using VShop.SharedKernel.Infrastructure.Extensions;
using VShop.SharedKernel.Infrastructure.Messaging.Events;
using VShop.SharedKernel.EventSourcing.Repositories;

namespace VShop.SharedKernel.EventStore.Repositories
{
    public class EventStoreIntegrationRepository : IIntegrationRepository
    {
        private readonly IEventStoreConnection _eventStoreConnection;
        private readonly string _integrationStreamName;

        public EventStoreIntegrationRepository(IEventStoreConnection eventStoreConnection)
        {
            _eventStoreConnection = eventStoreConnection;
            _integrationStreamName = $"{_eventStoreConnection.ConnectionName}/integration".ToSnakeCase();
        }
        
        public async Task SaveAsync(IIntegrationEvent @event)
        {
            if (@event is null)
                throw new ArgumentNullException(nameof(@event));

            await _eventStoreConnection.AppendToStreamAsync
            (
                _integrationStreamName,
                ExpectedVersion.Any,
                 @event
            );
        }

        public async Task<IEnumerable<IIntegrationEvent>> LoadAsync()
        {
            List<IIntegrationEvent> events = await _eventStoreConnection
                .ReadStreamEventsForwardAsync<IIntegrationEvent>(_integrationStreamName);

            return events.AsEnumerable();
        }
    }
}