using Xunit;

using VShop.SharedKernel.Subscriptions;
using VShop.SharedKernel.Tests.IntegrationTests.Contracts;

namespace VShop.Tests.IntegrationTests.Infrastructure;

public class TestBase : IAsyncLifetime
{
    protected static IModuleFixture SalesModule => IntegrationTestsFixture.SalesModule;
    protected static IModuleFixture ProcessManagerModule => IntegrationTestsFixture.ProcessManagerModule;
    protected static IModuleFixture BillingModule => IntegrationTestsFixture.BillingModule;
    protected static IModuleFixture CatalogModule => IntegrationTestsFixture.CatalogModule;

    public async Task InitializeAsync()
    {
        await SalesModule.StartHostedServicesAsync();
        await ProcessManagerModule.StartHostedServicesAsync();
        await BillingModule.StartHostedServicesAsync();
        await CatalogModule.StartHostedServicesAsync();

        await Task.WhenAll(ModuleEventStoreSubscriptionRegistry.Services
            .Select(s => s.StartAsync(CancellationToken.None)));
    }

    public async Task DisposeAsync()
    {
        await SalesModule.StopHostedServicesAsync();
        await ProcessManagerModule.StopHostedServicesAsync();
        await BillingModule.StopHostedServicesAsync();
        await CatalogModule.StopHostedServicesAsync();
        
        await Task.WhenAll(ModuleEventStoreSubscriptionRegistry.Services
            .Select(s => s.StopAsync(CancellationToken.None)));
    }
}