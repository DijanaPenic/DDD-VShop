using Xunit;

namespace VShop.Modules.Sales.Tests.IntegrationTests.Infrastructure
{
    internal class SchedulerFixture : IAsyncLifetime
    {
        private const string HostedServiceName = "QuartzHostedService";

        public async Task InitializeAsync()
        {
            // Scheduler might pick up data from other tests, so we need to clear databases.
            await ResetDatabaseLifetime.StartDatabaseResetAsync();
            
            await IntegrationTestsFixture.ExecuteHostedServiceAsync
                (hostedService => hostedService.StartAsync(CancellationToken.None), HostedServiceName);
        }

        public Task DisposeAsync()
            => IntegrationTestsFixture.ExecuteHostedServiceAsync
                (hostedService => hostedService.StopAsync(CancellationToken.None), HostedServiceName);
    }
}