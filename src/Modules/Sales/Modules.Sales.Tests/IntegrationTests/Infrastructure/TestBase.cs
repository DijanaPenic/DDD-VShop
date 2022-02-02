using VShop.SharedKernel.Tests.IntegrationTests.Contracts;

namespace VShop.Modules.Sales.Tests.IntegrationTests.Infrastructure;

public abstract class TestBase
{
    protected static IModuleFixture SalesModule => IntegrationTestsFixture.SalesModule;
}