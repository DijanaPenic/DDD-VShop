using Microsoft.Extensions.Hosting;

using VShop.SharedKernel.EventStoreDb.Subscriptions.Services.Contracts;

namespace VShop.SharedKernel.EventStoreDb.Subscriptions.Services
{
    public class SubscriptionHostedService : IHostedService
    {
        private readonly IEnumerable<ISubscriptionBackgroundService> _subscribeBackgroundServices;

        public SubscriptionHostedService(IEnumerable<ISubscriptionBackgroundService> subscribeBackgroundServices)
            => _subscribeBackgroundServices = subscribeBackgroundServices;

        public async Task StartAsync(CancellationToken cancellationToken)
            => await Task.WhenAll(_subscribeBackgroundServices.Select(s => s.StartAsync(cancellationToken)));

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            foreach(ISubscriptionBackgroundService esSubscriptionManager in _subscribeBackgroundServices)
                await esSubscriptionManager.StopAsync(cancellationToken);
        }
    }
}