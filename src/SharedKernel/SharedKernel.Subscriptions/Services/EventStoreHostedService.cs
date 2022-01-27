using Microsoft.Extensions.Hosting;

using VShop.SharedKernel.Subscriptions.Services.Contracts;

namespace VShop.SharedKernel.Subscriptions.Services
{
    public class EventStoreHostedService : IHostedService
    {
        private readonly IList<IEventStoreBackgroundService> _backgroundServices;

        public EventStoreHostedService(IList<IEventStoreBackgroundService> backgroundServices)
            => _backgroundServices = backgroundServices;

        public async Task StartAsync(CancellationToken cancellationToken)
            => await Task.WhenAll(_backgroundServices.Select(s => s.StartAsync(cancellationToken)));

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            foreach(IEventStoreBackgroundService backgroundService in _backgroundServices)
                await backgroundService.StopAsync(cancellationToken);
        }
    }
}