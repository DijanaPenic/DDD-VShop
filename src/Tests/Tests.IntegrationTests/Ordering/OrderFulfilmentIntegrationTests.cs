using Xunit;
using FluentAssertions;

using VShop.Modules.Billing.Integration.Events;
using VShop.Modules.Sales.Domain.Enums;
using VShop.Modules.Sales.Domain.Models.Ordering;
using VShop.Modules.Sales.Domain.Models.ShoppingCart;
using VShop.Modules.Sales.Infrastructure.Commands;
using VShop.Modules.Sales.Infrastructure.Commands.Handlers;
using VShop.SharedKernel.Domain.ValueObjects;
using VShop.SharedKernel.Infrastructure;
using VShop.SharedKernel.Tests.Customizations;
using VShop.Tests.IntegrationTests.Helpers;
using VShop.Tests.IntegrationTests.Infrastructure;

namespace VShop.Tests.IntegrationTests.Ordering;

public class OrderFulfilmentIntegrationTests : TestBase
{
    [Theory, CustomAutoData]
    internal async Task Test(ShoppingCart shoppingCart)
    {
        // TODO
        // * need to add products to catalog
        // * need to check that catalog count was decreased
        // 
        
        
        // Arrange
        await ShoppingCartHelper.SaveAndPublishAsync(shoppingCart);
    
        CheckoutShoppingCartCommand command = new(shoppingCart.Id);
        
        // Act
        Result<CheckoutResponse> result = await SalesModule.SendAsync(command);
        
        // Assert
        result.IsError.Should().BeFalse();
        
        // The shopping cart should have been closed.
        ShoppingCart shoppingCartFromDb = await ShoppingCartHelper.GetShoppingCartAsync(shoppingCart.Id);
        shoppingCartFromDb.Should().NotBeNull();
        shoppingCartFromDb.Status.Should().Be(ShoppingCartStatus.Closed);
        
        // The order should have been created.
        EntityId orderId = EntityId.Create(result.Data.OrderId).Data;
    
        Order orderFromDb = await OrderHelper.GetOrderAsync(orderId);
        orderFromDb.Should().NotBeNull();
        
        //**********************************//
        
        // Arrange
        PaymentSucceededIntegrationEvent paymentSucceededIntegrationEvent = new(orderId);
        
        // Act
        await ProcessManagerModule.PublishAsync(paymentSucceededIntegrationEvent);
        
        // Assert
        orderFromDb = await OrderHelper.GetOrderAsync(orderId);
        orderFromDb.Status.Should().Be(OrderStatus.Paid);
    }
}