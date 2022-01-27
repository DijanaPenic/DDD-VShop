using Xunit;

using VShop.SharedKernel.Subscriptions;

namespace VShop.Modules.Sales.Tests.IntegrationTests.Infrastructure
{
    internal class SubscriptionFixture : IAsyncLifetime
    {
        public async Task InitializeAsync()
        {
            // Subscriptions might pick up data from other tests, so we need to clear databases.
            // Checkpoints might not be in sync with the EventStore database - another reason for cleanup.
            await ResetDatabaseLifetime.StartDatabaseResetAsync();

            await Task.WhenAll(ModuleEventStoreSubscriptionRegistry.Services
                .Select(s => s.StartAsync(CancellationToken.None)));
        }
        public async Task DisposeAsync()
            => await Task.WhenAll(ModuleEventStoreSubscriptionRegistry.Services
                .Select(s => s.StopAsync(CancellationToken.None)));
    }
}