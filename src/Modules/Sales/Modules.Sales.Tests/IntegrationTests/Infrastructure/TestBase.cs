using Xunit;

using VShop.SharedKernel.Subscriptions;
using VShop.SharedKernel.Tests.IntegrationTests.Contracts;

namespace VShop.Modules.Sales.Tests.IntegrationTests.Infrastructure;

public class TestBase : IAsyncLifetime
{
    protected static IModuleFixture SalesModule => IntegrationTestsFixture.SalesModule;
    
    public async Task InitializeAsync()
    {
        await SalesModule.StartHostedServicesAsync();

        await Task.WhenAll(ModuleEventStoreSubscriptionRegistry.Services
            .Select(s => s.StartAsync(CancellationToken.None)));
    }

    public async Task DisposeAsync()
    {
        await SalesModule.StopHostedServicesAsync();

        await Task.WhenAll(ModuleEventStoreSubscriptionRegistry.Services
            .Select(s => s.StopAsync(CancellationToken.None)));
    }
}