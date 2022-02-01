using VShop.SharedKernel.Tests.IntegrationTests.Contracts;

namespace VShop.Modules.ProcessManager.Tests.IntegrationTests.Infrastructure;

public abstract class TestBase
{
    public static IModuleFixture ProcessManagerModule => IntegrationTestsFixture.ProcessManagerModule;
}