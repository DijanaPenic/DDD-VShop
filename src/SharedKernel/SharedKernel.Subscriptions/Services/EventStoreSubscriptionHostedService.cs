using Microsoft.Extensions.Hosting;

using VShop.SharedKernel.Infrastructure.Services.Contracts;

namespace VShop.SharedKernel.Subscriptions.Services
{
    public class EventStoreSubscriptionHostedService : IHostedService
    {
        private readonly IEnumerable<ISubscriptionBackgroundService> _subscribeBackgroundServices;

        public EventStoreSubscriptionHostedService(IEnumerable<ISubscriptionBackgroundService> subscribeBackgroundServices)
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