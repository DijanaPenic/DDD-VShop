using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Hosting;

using VShop.SharedKernel.EventStore.Subscriptions.Contracts;

namespace VShop.SharedKernel.EventStore
{
    public class EventStoreService : IHostedService
    {
        private readonly IEnumerable<IEventStoreSubscriptionManager> _eventStoreSubscriptionManagers;

        public EventStoreService(IEnumerable<IEventStoreSubscriptionManager> eventStoreSubscriptionManagers)
            => _eventStoreSubscriptionManagers = eventStoreSubscriptionManagers;

        public async Task StartAsync(CancellationToken cancellationToken)
            => await Task.WhenAll(_eventStoreSubscriptionManagers.Select(sm => sm.StartAsync()));

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            foreach(IEventStoreSubscriptionManager esSubscriptionManager in _eventStoreSubscriptionManagers)
                await esSubscriptionManager.StopAsync();
        }
    }
}