using VShop.SharedKernel.Tests.IntegrationTests.Contracts;

namespace VShop.Modules.Sales.Tests.IntegrationTests.Infrastructure;

public abstract class TestBase
{
    public static IModuleFixture SalesModule => IntegrationTestsFixture.SalesModule;
}