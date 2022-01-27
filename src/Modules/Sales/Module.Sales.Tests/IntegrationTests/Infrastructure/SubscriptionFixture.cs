using Xunit;

using VShop.SharedKernel.Subscriptions.Services;

namespace VShop.Modules.Sales.Tests.IntegrationTests.Infrastructure
{
    internal class SubscriptionFixture : IAsyncLifetime
    {
        public async Task InitializeAsync()
        {
            // Subscriptions might pick up data from other tests, so we need to clear databases.
            // Checkpoints might not be in sync with the EventStore database - another reason for cleanup.
            await ResetDatabaseLifetime.StartDatabaseResetAsync();
            
            await IntegrationTestsFixture.ExecuteHostedServiceAsync<EventStoreHostedService>
                (hostedService => hostedService.StartAsync(CancellationToken.None));
        }
        
        public Task DisposeAsync()
            => IntegrationTestsFixture.ExecuteHostedServiceAsync<EventStoreHostedService>
                (hostedService => hostedService.StopAsync(CancellationToken.None));
    }
}