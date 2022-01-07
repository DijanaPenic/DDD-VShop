using Xunit;
using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;

using VShop.SharedKernel.Domain.Enums;
using VShop.SharedKernel.Domain.ValueObjects;
using VShop.SharedKernel.Infrastructure;
using VShop.Modules.Sales.Domain.Enums;
using VShop.Modules.Sales.Domain.Models.Ordering;
using VShop.Modules.Sales.Domain.Models.ShoppingCart;
using VShop.Modules.Sales.Tests.Customizations;
using VShop.Modules.Sales.API.Application.Commands;
using VShop.Modules.Sales.API.Application.Commands.Shared;
using VShop.Modules.Sales.API.Tests.IntegrationTests.Helpers;
using VShop.Modules.Sales.API.Tests.IntegrationTests.Infrastructure;

namespace VShop.Modules.Sales.API.Tests.IntegrationTests
{
    [Collection("Non-Parallel Tests Collection")]
    public class ShoppingCartCommandHandlerIntegrationTests
    {
        [Theory]
        [CustomizedAutoData]
        public async Task Creates_a_new_shopping_cart
        (
            EntityId shoppingCartId,
            EntityId customerId,
            Discount customerDiscount,
            AddShoppingCartItem[] shoppingCartItems
        )
        {
            // Arrange
            CreateShoppingCartCommand command = new
            (
                shoppingCartId,
                customerId,
                customerDiscount,
                shoppingCartItems
            );

            // Act
            Result<ShoppingCart> result = await IntegrationTestsFixture.SendAsync(command);
            
            // Assert
            result.IsError.Should().BeFalse();

            ShoppingCart shoppingCartFromDb = await ShoppingCartHelper.GetShoppingCartAsync(shoppingCartId);
            shoppingCartFromDb.Should().NotBeNull();
        }
        
        [Theory]
        [CustomizedAutoData]
        public async Task Shopping_cart_creation_command_is_idempotent
        (
            EntityId shoppingCartId,
            EntityId customerId,
            Discount customerDiscount,
            AddShoppingCartItem[] shoppingCartItems
        )
        {
            // Arrange
            CreateShoppingCartCommand command = new
            (
                shoppingCartId,
                customerId,
                customerDiscount,
                shoppingCartItems
            );
            await IntegrationTestsFixture.SendAsync(command);

            // Act
            Result<ShoppingCart> result = await IntegrationTestsFixture.SendAsync(command);
            
            // Assert
            result.IsError.Should().BeFalse();
        }
        
        [Theory]
        [CustomizedAutoData]
        public async Task Changes_product_price_in_the_shopping_cart(ShoppingCart shoppingCart)
        {
            // Arrange
            await ShoppingCartHelper.SaveAndPublishAsync(shoppingCart);
            ShoppingCartItem shoppingCartItem = shoppingCart.Items[0];

            SetShoppingCartProductPriceCommand command = new
            (
                shoppingCart.Id,
                shoppingCartItem.Id,
                shoppingCartItem.UnitPrice.Value + 1
            );
            
            // Act
            Result result = await IntegrationTestsFixture.SendAsync(command);
            
            // Assert
            result.IsError.Should().BeFalse();
            
            ShoppingCart shoppingCartFromDb = await ShoppingCartHelper.GetShoppingCartAsync(shoppingCart.Id);
            shoppingCartFromDb.Should().NotBeNull();
            
            ShoppingCartItem shoppingCartItemFromDb = shoppingCartFromDb.Items
                .SingleOrDefault(sci => sci.Id.Value == command.ProductId);
            shoppingCartItemFromDb.Should().NotBeNull();
            shoppingCartItemFromDb!.UnitPrice.Value.Should().Be(command.UnitPrice);
        }
        
        [Theory]
        [CustomizedAutoData]
        public async Task Changing_product_price_in_the_shopping_cart_is_idempotent(ShoppingCart shoppingCart)
        {
            // Arrange
            await ShoppingCartHelper.SaveAndPublishAsync(shoppingCart);
            ShoppingCartItem shoppingCartItem = shoppingCart.Items[0];

            SetShoppingCartProductPriceCommand command = new
            (
                shoppingCart.Id,
                shoppingCartItem.Id,
                shoppingCartItem.UnitPrice.Value + 1
            );
            
            await IntegrationTestsFixture.SendAsync(command);
            
            // Act
            Result result = await IntegrationTestsFixture.SendAsync(command);
            
            // Assert
            result.IsError.Should().BeFalse();
        }
        
        [Theory]
        [CustomizedAutoData]
        public async Task Adds_a_new_product_to_the_shopping_cart
        (
            ShoppingCart shoppingCart, 
            AddShoppingCartItem shoppingCartItem
        )
        {
            // Arrange
            await ShoppingCartHelper.SaveAndPublishAsync(shoppingCart);
            
            AddShoppingCartProductCommand command = new
            (
                shoppingCart.Id,
                shoppingCartItem
            );
            
            // Act
            Result result = await IntegrationTestsFixture.SendAsync(command);
            
            // Assert
            result.IsError.Should().BeFalse();
            
            ShoppingCart shoppingCartFromDb = await ShoppingCartHelper.GetShoppingCartAsync(shoppingCart.Id);
            shoppingCartFromDb.Should().NotBeNull();
            
            ShoppingCartItem shoppingCartItemFromDb = shoppingCartFromDb.Items
                .SingleOrDefault(sci => sci.Id.Value == command.ShoppingCartItem.ProductId);
            shoppingCartItemFromDb.Should().NotBeNull();
        }
        
        [Theory]
        [CustomizedAutoData]
        public async Task Adding_a_new_product_to_the_shopping_cart_command_is_idempotent
        (
            ShoppingCart shoppingCart, 
            AddShoppingCartItem shoppingCartItem
        )
        {
            // Arrange
            await ShoppingCartHelper.SaveAndPublishAsync(shoppingCart);
            
            AddShoppingCartProductCommand command = new
            (
                shoppingCart.Id,
                shoppingCartItem
            );
            
            await IntegrationTestsFixture.SendAsync(command);
            
            // Act
            Result result = await IntegrationTestsFixture.SendAsync(command);
            
            // Assert
            result.IsError.Should().BeFalse();
        }
        
        [Theory]
        [CustomizedAutoData]
        public async Task Removes_a_product_from_the_shopping_cart(ShoppingCart shoppingCart)
        {
            // Arrange
            await ShoppingCartHelper.SaveAndPublishAsync(shoppingCart);
            ShoppingCartItem shoppingCartItem = shoppingCart.Items[0];
            
            RemoveShoppingCartProductCommand command = new
            (
                shoppingCart.Id,
                shoppingCartItem.Id,
                shoppingCartItem.Quantity
            );
            
            // Act
            Result result = await IntegrationTestsFixture.SendAsync(command);
            
            // Assert
            result.IsError.Should().BeFalse();
            
            ShoppingCart shoppingCartFromDb = await ShoppingCartHelper.GetShoppingCartAsync(shoppingCart.Id);
            shoppingCartFromDb.Should().NotBeNull();
            
            ShoppingCartItem shoppingCartItemFromDb = shoppingCartFromDb.Items
                .SingleOrDefault(sci => sci.Id.Value == command.ProductId);
            shoppingCartItemFromDb.Should().BeNull();
        }
        
        [Theory]
        [CustomizedAutoData]
        public async Task Removing_a_product_from_the_shopping_cart_command_is_idempotent(ShoppingCart shoppingCart)
        {
            // Arrange
            await ShoppingCartHelper.SaveAndPublishAsync(shoppingCart);
            ShoppingCartItem shoppingCartItem = shoppingCart.Items[0];
            
            RemoveShoppingCartProductCommand command = new
            (
                shoppingCart.Id,
                shoppingCartItem.Id,
                shoppingCartItem.Quantity
            );
            await IntegrationTestsFixture.SendAsync(command);
            
            // Act
            Result result = await IntegrationTestsFixture.SendAsync(command);
            
            // Assert
            result.IsError.Should().BeFalse();
        }
        
        [Theory]
        [CustomizedAutoData]
        public async Task Sets_a_customer_contact_information
        (
            ShoppingCart shoppingCart,
            FullName fullName,
            GenderType gender,
            EmailAddress emailAddress,
            PhoneNumber phoneNumber
        )
        {
            // Arrange
            await ShoppingCartHelper.SaveAndPublishAsync(shoppingCart);
            
            SetContactInformationCommand command = new
            (
                shoppingCart.Id,
                fullName.FirstName,
                fullName.MiddleName,
                fullName.LastName,
                emailAddress,
                phoneNumber,
                gender
            );
            
            // Act
            Result result = await IntegrationTestsFixture.SendAsync(command);
            
            // Assert
            result.IsError.Should().BeFalse();
            
            ShoppingCart shoppingCartFromDb = await ShoppingCartHelper.GetShoppingCartAsync(shoppingCart.Id);
            shoppingCartFromDb.Should().NotBeNull();
            shoppingCartFromDb.Customer.FullName.Should().Be(fullName);
            shoppingCartFromDb.Customer.Gender.Should().Be(gender);
            shoppingCartFromDb.Customer.EmailAddress.Should().Be(emailAddress);
            shoppingCartFromDb.Customer.PhoneNumber.Should().Be(phoneNumber);
        }
        
        [Theory]
        [CustomizedAutoData]
        public async Task Setting_a_customer_contact_information_command_is_idempotent
        (
            ShoppingCart shoppingCart,
            FullName fullName,
            GenderType gender,
            EmailAddress emailAddress,
            PhoneNumber phoneNumber
        )
        {
            // Arrange
            await ShoppingCartHelper.SaveAndPublishAsync(shoppingCart);
            
            SetContactInformationCommand command = new
            (
                shoppingCart.Id,
                fullName.FirstName,
                fullName.MiddleName,
                fullName.LastName,
                emailAddress,
                phoneNumber,
                gender
            );
            await IntegrationTestsFixture.SendAsync(command);
            
            // Act
            Result result = await IntegrationTestsFixture.SendAsync(command);
            
            // Assert
            result.IsError.Should().BeFalse();
        }
        
        [Theory]
        [CustomizedAutoData]
        public async Task Sets_a_customer_delivery_address
        (
            ShoppingCart shoppingCart,
            Address deliveryAddress
        )
        {
            // Arrange
            await ShoppingCartHelper.SaveAndPublishAsync(shoppingCart);
            
            SetDeliveryAddressCommand command = new
            (
                shoppingCart.Id,
                deliveryAddress.City,
                deliveryAddress.CountryCode,
                deliveryAddress.PostalCode,
                deliveryAddress.StateProvince,
                deliveryAddress.StreetAddress
            );
            
            // Act
            Result result = await IntegrationTestsFixture.SendAsync(command);
            
            // Assert
            result.IsError.Should().BeFalse();
            
            ShoppingCart shoppingCartFromDb = await ShoppingCartHelper.GetShoppingCartAsync(shoppingCart.Id);
            shoppingCartFromDb.Should().NotBeNull();
            shoppingCartFromDb.Customer.DeliveryAddress.Should().Be(deliveryAddress);
        }
        
        [Theory]
        [CustomizedAutoData]
        public async Task Setting_a_customer_delivery_address_command_is_idempotent
        (
            ShoppingCart shoppingCart,
            Address deliveryAddress
        )
        {
            // Arrange
            await ShoppingCartHelper.SaveAndPublishAsync(shoppingCart);
            
            SetDeliveryAddressCommand command = new
            (
                shoppingCart.Id,
                deliveryAddress.City,
                deliveryAddress.CountryCode,
                deliveryAddress.PostalCode,
                deliveryAddress.StateProvince,
                deliveryAddress.StreetAddress
            );
            
            // Act
            Result result = await IntegrationTestsFixture.SendAsync(command);
            
            // Assert
            result.IsError.Should().BeFalse();
        }
        
        [Theory]
        [CustomizedAutoData]
        public async Task Deletes_the_shopping_cart(ShoppingCart shoppingCart)
        {
            // Arrange
            await ShoppingCartHelper.SaveAndPublishAsync(shoppingCart);
            
            DeleteShoppingCartCommand command = new(shoppingCart.Id);
            
            // Act
            Result result = await IntegrationTestsFixture.SendAsync(command);
            
            // Assert
            result.IsError.Should().BeFalse();
            
            ShoppingCart shoppingCartFromDb = await ShoppingCartHelper.GetShoppingCartAsync(shoppingCart.Id);
            shoppingCartFromDb.Status.Should().Be(ShoppingCartStatus.Closed);
        }
        
        [Theory]
        [CustomizedAutoData]
        public async Task Deleting_the_shopping_cart_command_is_idempotent(ShoppingCart shoppingCart)
        {
            // Arrange
            await ShoppingCartHelper.SaveAndPublishAsync(shoppingCart);
            
            DeleteShoppingCartCommand command = new(shoppingCart.Id);
            
            await IntegrationTestsFixture.SendAsync(command);
            
            // Act
            Result result = await IntegrationTestsFixture.SendAsync(command);
            
            // Assert
            result.IsError.Should().BeFalse();
        }
        
         [Theory]
         [CustomizedAutoData]
         public async Task Shopping_cart_checkout_places_an_order(ShoppingCart shoppingCart)
         {
             // Arrange
             await ShoppingCartHelper.SaveAndPublishAsync(shoppingCart);

             CheckoutShoppingCartCommand command = new(shoppingCart.Id);
             
             // Act
             Result<CheckoutResponse> result = await IntegrationTestsFixture.SendAsync(command);
             
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
         
         [Theory]
         [CustomizedAutoData]
         public async Task Shopping_cart_checkout_command_is_idempotent(ShoppingCart shoppingCart)
         {
             // Arrange
             await ShoppingCartHelper.SaveAndPublishAsync(shoppingCart);

             CheckoutShoppingCartCommand command = new(shoppingCart.Id);
             
             await IntegrationTestsFixture.SendAsync(command);
             
             // Act
             Result<CheckoutResponse> result = await IntegrationTestsFixture.SendAsync(command);
             
             // Assert
             result.IsError.Should().BeFalse();
         }
    }
}