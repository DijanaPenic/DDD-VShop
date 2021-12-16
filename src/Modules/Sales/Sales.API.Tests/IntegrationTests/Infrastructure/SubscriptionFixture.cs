using Xunit;
using System.Threading;
using System.Threading.Tasks;

using VShop.SharedKernel.EventStoreDb.Subscriptions.Services;

namespace VShop.Modules.Sales.API.Tests.IntegrationTests.Infrastructure
{
    public class SubscriptionFixture : IAsyncLifetime
    {
        public async Task InitializeAsync()
        {
            // Subscriptions might pick up data from other tests, so we need to clear databases.
            // Checkpoints might not be in sync with the EventStore database - another reason for cleanup.
            await ResetDatabaseLifetime.StartDatabaseResetAsync();
            
            await IntegrationTestsFixture.ExecuteHostedServiceAsync<SubscriptionHostedService>
                (hostedService => hostedService.StartAsync(CancellationToken.None));
        }
        
        public Task DisposeAsync()
            => IntegrationTestsFixture.ExecuteHostedServiceAsync<SubscriptionHostedService>
                (hostedService => hostedService.StopAsync(CancellationToken.None));
    }
}