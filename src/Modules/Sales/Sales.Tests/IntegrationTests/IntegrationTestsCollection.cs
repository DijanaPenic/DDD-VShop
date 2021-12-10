using Xunit;

namespace VShop.Modules.Sales.Tests.IntegrationTests
{
    [CollectionDefinition("Integration Tests Collection")]
    public class IntegrationTestsCollection : ICollectionFixture<AppFixture>
    {
        
    }
}