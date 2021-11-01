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

        public EventStoreIntegrationRepository(IEventStoreConnection esConnection)
        {
            _esConnection = esConnection;
        }
        
        public async Task SaveAsync(IIntegrationEvent @event)
        {
            if (@event is null)
                throw new ArgumentNullException(nameof(@event));

            await _esConnection.AppendToStreamAsync
            (
                GetStreamName(),
                ExpectedVersion.Any,
                EventStoreHelper.PrepareEventData(messages: @event)
            );
        }

        public async Task<IEnumerable<IIntegrationEvent>> LoadAsync()
        {
            List<IIntegrationEvent> events = await _esConnection.ReadStreamEventsForwardAsync<IIntegrationEvent>(GetStreamName());

            return events.AsEnumerable();
        }

        private string GetStreamName() => $"{_esConnection.ConnectionName}/integration".ToSnakeCase();
    }
}