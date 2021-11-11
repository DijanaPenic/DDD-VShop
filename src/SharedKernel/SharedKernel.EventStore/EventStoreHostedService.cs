using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Hosting;

using VShop.SharedKernel.EventStore.Subscriptions;

namespace VShop.SharedKernel.EventStore
{
    public class EventStoreHostedService : IHostedService
    {
        private readonly IEnumerable<ISubscribeBackgroundService> _subscribeBackgroundServices;

        public EventStoreHostedService(IEnumerable<ISubscribeBackgroundService> subscribeBackgroundServices)
            => _subscribeBackgroundServices = subscribeBackgroundServices;

        public async Task StartAsync(CancellationToken cancellationToken)
            => await Task.WhenAll(_subscribeBackgroundServices.Select(s => s.StartAsync(cancellationToken)));

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            foreach(ISubscribeBackgroundService esSubscriptionManager in _subscribeBackgroundServices)
                await esSubscriptionManager.StopAsync(cancellationToken);
        }
    }
}