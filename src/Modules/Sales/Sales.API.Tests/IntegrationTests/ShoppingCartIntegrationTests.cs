using Xunit;
using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;

using VShop.SharedKernel.Domain.Enums;
using VShop.SharedKernel.Domain.ValueObjects;
using VShop.SharedKernel.Infrastructure;
using VShop.Modules.Sales.Tests.Customizations;
using VShop.Modules.Sales.API.Application.Commands;
using VShop.Modules.Sales.API.Application.Commands.Shared;
using VShop.Modules.Sales.API.Tests.IntegrationTests.Helpers;
using VShop.Modules.Sales.API.Tests.IntegrationTests.Infrastructure;
using VShop.Modules.Sales.Domain.Models.ShoppingCart;

namespace VShop.Modules.Sales.API.Tests.IntegrationTests
{
    [Collection("Non-Parallel Tests Collection")]
    public class ShoppingCartIntegrationTests
    {
        [Theory]
        [CustomizedAutoData]
        public async Task Crete_a_new_shopping_cart
        (
            Guid shoppingCartId,
            Guid customerId,
            Discount customerDiscount,
            ShoppingCartItemCommandDto[] shoppingCartItems
        )
        {
            // Arrange
            CreateShoppingCartCommand command = new()
            {
                ShoppingCartId = shoppingCartId,
                CustomerId = customerId,
                CustomerDiscount = customerDiscount,
                ShoppingCartItems = shoppingCartItems
            };

            // Act
            Result<ShoppingCart> result = await IntegrationTestsFixture.SendAsync(command);
            
            // Assert
            result.IsError(out _).Should().BeFalse();
            
            ShoppingCart shoppingCartFromDb = await ShoppingCartHelper.GetShoppingCartAsync(command.ShoppingCartId);
            shoppingCartFromDb.Should().NotBeNull();
        }
        
        [Theory]
        [CustomizedAutoData]
        public async Task Add_a_product_to_the_shopping_cart
        (
            ShoppingCart shoppingCart, 
            ShoppingCartItemCommandDto shoppingCartItem
        )
        {
            // Arrange
            await ShoppingCartHelper.SaveAndPublishAsync(shoppingCart);
            
            AddShoppingCartProductCommand command = new()
            {
                ShoppingCartId = shoppingCart.Id,
                ShoppingCartItem = shoppingCartItem
            };
            
            // Act
            Result result = await IntegrationTestsFixture.SendAsync(command);
            
            // Assert
            result.IsError(out _).Should().BeFalse();
            
            ShoppingCart shoppingCartFromDb = await ShoppingCartHelper.GetShoppingCartAsync(command.ShoppingCartId);
            ShoppingCartItem shoppingCartItemFromDb = shoppingCartFromDb.Items
                .SingleOrDefault(sci => sci.Id == command.ShoppingCartItem.ProductId);
            shoppingCartItemFromDb.Should().NotBeNull();
        }
        
        [Theory]
        [CustomizedAutoData]
        public async Task Remove_a_product_from_the_shopping_cart(ShoppingCart shoppingCart)
        {
            // Arrange
            await ShoppingCartHelper.SaveAndPublishAsync(shoppingCart);
            ShoppingCartItem shoppingCartItem = shoppingCart.Items.First();
            
            RemoveShoppingCartProductCommand command = new()
            {
                ShoppingCartId = shoppingCart.Id,
                ProductId = shoppingCartItem.Id,
                Quantity = shoppingCartItem.Quantity
            };
            
            // Act
            Result result = await IntegrationTestsFixture.SendAsync(command);
            
            // Assert
            result.IsError(out _).Should().BeFalse();
            
            ShoppingCart shoppingCartFromDb = await ShoppingCartHelper.GetShoppingCartAsync(command.ShoppingCartId);
            ShoppingCartItem shoppingCartItemFromDb = shoppingCartFromDb.Items
                .SingleOrDefault(sci => sci.Id == command.ProductId);
            shoppingCartItemFromDb.Should().BeNull();
        }
        
        [Theory]
        [CustomizedAutoData]
        public async Task Set_customer_contact_information
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
            
            SetContactInformationCommand command = new()
            {
                ShoppingCartId = shoppingCart.Id,
                FirstName = fullName.FirstName,
                MiddleName = fullName.MiddleName,
                LastName = fullName.LastName,
                Gender = gender,
                EmailAddress = emailAddress,
                PhoneNumber = phoneNumber
            };
            
            // Act
            Result result = await IntegrationTestsFixture.SendAsync(command);
            
            // Assert
            result.IsError(out _).Should().BeFalse();

            ShoppingCart shoppingCartFromDb = await ShoppingCartHelper.GetShoppingCartAsync(command.ShoppingCartId);
            shoppingCartFromDb.Customer.FullName.Should().Be(fullName);
            shoppingCartFromDb.Customer.Gender.Should().Be(gender);
            shoppingCartFromDb.Customer.EmailAddress.Should().Be(emailAddress);
            shoppingCartFromDb.Customer.PhoneNumber.Should().Be(phoneNumber);
        }
        
        [Theory]
        [CustomizedAutoData]
        public async Task Set_customer_delivery_address(ShoppingCart shoppingCart, Address deliveryAddress)
        {
            // Arrange
            await ShoppingCartHelper.SaveAndPublishAsync(shoppingCart);
            
            SetDeliveryAddressCommand command = new()
            {
                ShoppingCartId = shoppingCart.Id,
                City = deliveryAddress.City,
                CountryCode = deliveryAddress.CountryCode,
                PostalCode = deliveryAddress.PostalCode,
                StateProvince = deliveryAddress.StateProvince,
                StreetAddress = deliveryAddress.StreetAddress
            };
            
            // Act
            Result result = await IntegrationTestsFixture.SendAsync(command);
            
            // Assert
            result.IsError(out _).Should().BeFalse();
            
            ShoppingCart shoppingCartFromDb = await ShoppingCartHelper.GetShoppingCartAsync(command.ShoppingCartId);
            shoppingCartFromDb.Customer.DeliveryAddress.Should().Be(deliveryAddress);
        }
    }
}