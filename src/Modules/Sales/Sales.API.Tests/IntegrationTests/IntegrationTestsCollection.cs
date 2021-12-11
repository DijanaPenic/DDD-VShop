using Xunit;

using VShop.SharedKernel.Tests;

namespace VShop.Modules.Sales.API.Tests.IntegrationTests
{
    [CollectionDefinition("Integration Tests Collection")]
    public class IntegrationTestsCollection : ICollectionFixture<AppFixture>
    {
        
    }
}