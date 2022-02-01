using Xunit;

namespace VShop.Modules.ProcessManager.Tests.IntegrationTests.Infrastructure
{
    internal class SchedulerFixture : IAsyncLifetime
    {
        private const string HostedServiceName = "QuartzHostedService";

        public async Task InitializeAsync()
        {
            // Scheduler might pick up data from other tests, so we need to clear the databases first.
            await ResetDatabaseLifetime.ResetRelationalDatabaseAsync();
            
            await IntegrationTestsFixture.ProcessManagerModule.ExecuteHostedServiceAsync
                (hostedService => hostedService.StartAsync(CancellationToken.None), HostedServiceName);
        }

        public Task DisposeAsync()
            => IntegrationTestsFixture.ProcessManagerModule.ExecuteHostedServiceAsync
                (hostedService => hostedService.StopAsync(CancellationToken.None), HostedServiceName);
    }
}