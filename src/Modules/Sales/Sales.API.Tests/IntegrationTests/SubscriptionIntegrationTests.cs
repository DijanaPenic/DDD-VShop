// using Xunit;
// using AutoFixture;
// using System.Threading;
// using System.Threading.Tasks;
//
// using VShop.SharedKernel.Tests;
// using VShop.SharedKernel.EventStoreDb.Subscriptions.Services;
//
// namespace VShop.Modules.Sales.API.Tests.IntegrationTests
// {
//     // TODO - work in progress
//     [Collection("Integration Tests Collection")]
//     public class SubscriptionIntegrationTests : IntegrationTestsBase
//     {
//         private readonly Fixture _autoFixture;
//
//         public SubscriptionIntegrationTests(AppFixture appFixture) => _autoFixture = appFixture.AutoFixture;
//
//         [Fact]
//         public Task Test()
//         {
//             // Arrange
//             SubscriptionHostedService subscriptionHostedService = GetSubscriptionHostedService();
//             subscriptionHostedService.StartAsync(CancellationToken.None);
//
//             // Act
//
//             // Assert
//             subscriptionHostedService.StopAsync(CancellationToken.None);
//             
//             return Task.CompletedTask;
//         }
//     }
// }