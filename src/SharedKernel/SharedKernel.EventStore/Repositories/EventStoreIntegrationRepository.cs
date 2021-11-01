using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using EventStore.ClientAPI;

using VShop.SharedKernel.EventStore.Helpers;
using VShop.SharedKernel.EventStore.Extensions;
using VShop.SharedKernel.EventStore.Repositories.Contracts;
using VShop.SharedKernel.Infrastructure.Messaging;
using VShop.SharedKernel.Infrastructure.Extensions;

namespace VShop.SharedKernel.EventStore.Repositories
{
    public class EventStoreIntegrationRepository : IEventStoreIntegrationRepository
    {
        private readonly IEventStoreConnection _esConnection;
        private readonly string _esIntegrationStreamName;

        public EventStoreIntegrationRepository(IEventStoreConnection esConnection)
        {
            _esConnection = esConnection;
            _esIntegrationStreamName = $"{_esConnection.ConnectionName}/integration".ToSnakeCase();
        }
        
        public async Task SaveAsync(IIntegrationEvent @event)
        {
            if (@event is null)
                throw new ArgumentNullException(nameof(@event));

            await _esConnection.AppendToStreamAsync
            (
                _esIntegrationStreamName,
                ExpectedVersion.Any,
                EventStoreHelper.PrepareEventData(messages: @event)
            );
        }

        public async Task<IEnumerable<IIntegrationEvent>> LoadAsync()
        {
            List<IIntegrationEvent> events = await _esConnection.ReadStreamEventsForwardAsync<IIntegrationEvent>(_esIntegrationStreamName);

            return events.AsEnumerable();
        }
    }
}