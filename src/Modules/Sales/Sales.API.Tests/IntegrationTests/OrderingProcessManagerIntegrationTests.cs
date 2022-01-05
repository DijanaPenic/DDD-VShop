using Xunit;
using FluentAssertions;
using System.Threading.Tasks;

using VShop.Modules.Sales.API.Application.ProcessManagers;
using VShop.Modules.Sales.API.Tests.IntegrationTests.Helpers;
using VShop.Modules.Sales.API.Tests.IntegrationTests.Infrastructure;
using VShop.Modules.Sales.Domain.Events;
using VShop.Modules.Sales.Tests.Customizations;
using VShop.SharedKernel.Domain.ValueObjects;
using VShop.SharedKernel.Infrastructure.Services;
using VShop.SharedKernel.Infrastructure.Services.Contracts;

namespace VShop.Modules.Sales.API.Tests.IntegrationTests
{
    public class OrderingProcessManagerIntegrationTests
    {
        [Theory]
        [CustomizedAutoData]
        public async Task Process_manager_transition
        (
            EntityId shoppingCartId,
            EntityId orderId
        )
        {
            // Arrange
            IClockService clockService = new ClockService();
            
            ShoppingCartCheckoutRequestedDomainEvent shoppingCartCheckoutRequestedDomainEvent = new()
            {
                ShoppingCartId = shoppingCartId,
                OrderId = orderId,
                ConfirmedAt = clockService.Now
            };
        
            // Act
            await IntegrationTestsFixture.PublishAsync(shoppingCartCheckoutRequestedDomainEvent);
            
            // Assert
            OrderingProcessManager processManagerFromDb = await OrderHelper.GetProcessManagerAsync(orderId);
            processManagerFromDb.Should().NotBeNull();
        }
    }
}