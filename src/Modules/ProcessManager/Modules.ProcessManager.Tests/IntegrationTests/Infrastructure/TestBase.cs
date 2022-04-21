using Xunit;

using VShop.SharedKernel.Subscriptions;
using VShop.SharedKernel.Tests.IntegrationTests.Contracts;

namespace VShop.Modules.ProcessManager.Tests.IntegrationTests.Infrastructure;

public abstract class TestBase : IAsyncLifetime
{
    protected static IModuleFixture ProcessManagerModule => IntegrationTestsFixture.ProcessManagerModule;
    
    public async Task InitializeAsync()
    {
        await ProcessManagerModule.StartHostedServicesAsync();

        await Task.WhenAll(ModuleEventStoreSubscriptionRegistry.Services
            .Select(s => s.StartAsync(CancellationToken.None)));
    }

    public async Task DisposeAsync()
    {
        await ProcessManagerModule.StopHostedServicesAsync();

        await Task.WhenAll(ModuleEventStoreSubscriptionRegistry.Services
            .Select(s => s.StopAsync(CancellationToken.None)));
    }
}