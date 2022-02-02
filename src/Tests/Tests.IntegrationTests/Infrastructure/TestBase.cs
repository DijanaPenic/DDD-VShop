using VShop.SharedKernel.Tests.IntegrationTests.Contracts;

namespace VShop.Tests.IntegrationTests.Infrastructure;

public abstract class TestBase
{
    protected static IModuleFixture SalesModule => IntegrationTestsFixture.SalesModule;
    protected static IModuleFixture ProcessManagerModule => IntegrationTestsFixture.ProcessManagerModule;
    protected static IModuleFixture BillingModule => IntegrationTestsFixture.BillingModule;
    protected static IModuleFixture CatalogModule => IntegrationTestsFixture.CatalogModule;
}