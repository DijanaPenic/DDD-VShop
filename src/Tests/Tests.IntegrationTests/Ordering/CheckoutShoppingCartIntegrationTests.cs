using Xunit;
using FluentAssertions;

using VShop.Modules.Sales.Domain.Enums;
using VShop.Modules.Sales.Domain.Models.Ordering;
using VShop.Modules.Sales.Domain.Models.ShoppingCart;
using VShop.Modules.Sales.Infrastructure.Commands;
using VShop.Modules.Sales.Infrastructure.Commands.Handlers;
using VShop.SharedKernel.Domain.ValueObjects;
using VShop.SharedKernel.Infrastructure;
using VShop.SharedKernel.Infrastructure.Contexts.Contracts;
using VShop.SharedKernel.Tests.Customizations;
using VShop.Tests.IntegrationTests.Helpers;
using VShop.Tests.IntegrationTests.Infrastructure;

namespace VShop.Tests.IntegrationTests.Ordering;

public class CheckoutShoppingCartIntegrationTests : TestBase
{
    [Theory, CustomAutoData]
    internal async Task Shopping_cart_checkout_places_an_order(ShoppingCart shoppingCart)
    {
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
    }
    
    [Theory, CustomAutoData]
    internal async Task Shopping_cart_checkout_command_is_idempotent
    (
        ShoppingCart shoppingCart,
        IContext context
    )
    {
        // Arrange
        await ShoppingCartHelper.SaveAndPublishAsync(shoppingCart);

        CheckoutShoppingCartCommand command = new(shoppingCart.Id);
             
        await SalesModule.SendAsync(command, context);
             
        // Act
        Result<CheckoutResponse> result = await SalesModule.SendAsync(command, context);
             
        // Assert
        result.IsError.Should().BeFalse();
    }
}