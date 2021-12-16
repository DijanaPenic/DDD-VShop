using Xunit;
using System.Threading.Tasks;

using VShop.Modules.Sales.API.Tests.IntegrationTests.Infrastructure;

namespace VShop.Modules.Sales.API.Tests.IntegrationTests
{
    [Collection("Integration Tests Collection")]
    public class SubscriptionIntegrationTests : ResetDatabaseLifetime, IClassFixture<SubscriptionFixture>
    {
        [Fact]
        public Task Test()
        {
            // Arrange
        
            // Act
        
            // Assert
            return Task.CompletedTask;
        }
    }
}