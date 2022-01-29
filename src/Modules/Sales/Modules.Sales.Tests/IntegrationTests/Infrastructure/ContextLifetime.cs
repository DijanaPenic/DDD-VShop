using Xunit;

namespace VShop.Modules.Sales.Tests.IntegrationTests.Infrastructure
{
    public abstract class ContextLifetime : IAsyncLifetime
    {
        public Task InitializeAsync() => IntegrationTestsFixture.SetContextAsync();
        public Task DisposeAsync() => Task.CompletedTask;
    }
}