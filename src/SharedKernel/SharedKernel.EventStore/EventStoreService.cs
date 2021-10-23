using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using EventStore.ClientAPI;
using Microsoft.Extensions.Hosting;

namespace VShop.SharedKernel.EventStore
{
    public class EventStoreService : IHostedService
    {
        private readonly IEventStoreConnection _esConnection;
        private readonly IEnumerable<EventStoreSubscriptionManager> _esSubscriptionManagers;

        public EventStoreService
        (
            IEventStoreConnection esConnection,
            IEnumerable<EventStoreSubscriptionManager> esSubscriptionManagers
        )
        {
            _esConnection = esConnection;
            _esSubscriptionManagers = esSubscriptionManagers;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _esConnection.ConnectAsync();
            await Task.WhenAll(_esSubscriptionManagers.Select(sm => sm.Start()));
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            foreach(EventStoreSubscriptionManager esSubscriptionManager in _esSubscriptionManagers)
            {
                esSubscriptionManager.Stop();
            }
            _esConnection.Close();
            
            return Task.CompletedTask;
        }
    }
}