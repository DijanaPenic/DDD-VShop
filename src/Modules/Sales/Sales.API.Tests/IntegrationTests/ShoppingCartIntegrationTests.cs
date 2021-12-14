using Xunit;
using System;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;

using VShop.SharedKernel.Tests;
using VShop.SharedKernel.Domain.Enums;
using VShop.SharedKernel.Domain.ValueObjects;
using VShop.SharedKernel.Infrastructure;
using VShop.Modules.Sales.API.Application.Commands;
using VShop.Modules.Sales.API.Application.Commands.Shared;
using VShop.Modules.Sales.API.Tests.IntegrationTests.Helpers;
using VShop.Modules.Sales.API.Tests.IntegrationTests.Infrastructure;
using VShop.Modules.Sales.Domain.Models.ShoppingCart;

namespace VShop.Modules.Sales.API.Tests.IntegrationTests
{
    [Collection("Integration Tests Collection")]
    public class ShoppingCartIntegrationTests : ResetDatabaseLifetime
    {
        private readonly Fixture _autoFixture;
        private readonly ShoppingCartHelper _shoppingCartHelper;

        public ShoppingCartIntegrationTests(AppFixture appFixture)
        {
            _autoFixture = appFixture.AutoFixture;
            _shoppingCartHelper = new ShoppingCartHelper(_autoFixture);
        }

        [Fact]
        public async Task Crete_a_new_shopping_cart()
        {
            // Arrange
            CreateShoppingCartCommand command = new()
            {
                ShoppingCartId = _autoFixture.Create<Guid>(),
                CustomerId = _autoFixture.Create<Guid>(),
                CustomerDiscount = _autoFixture.Create<Discount>(),
                ShoppingCartItems = _autoFixture.Create<ShoppingCartItemCommandDto[]>()
            };

            // Act
            Result<ShoppingCart> result = await IntegrationTestsFixture.SendAsync(command);
            
            // Assert
            result.IsError(out _).Should().BeFalse();
            
            ShoppingCart shoppingCartFromDb = await _shoppingCartHelper.GetShoppingCartAsync(command.ShoppingCartId);
            shoppingCartFromDb.Should().NotBeNull();
        }
        
        [Fact]
        public async Task Add_a_product_to_the_shopping_cart()
        {
            // Arrange
            ShoppingCart shoppingCart = await _shoppingCartHelper.PrepareShoppingCartForCheckoutAsync();

            AddShoppingCartProductCommand command = new()
            {
                ShoppingCartId = shoppingCart.Id,
                ShoppingCartItem = _autoFixture.Create<ShoppingCartItemCommandDto>()
            };
            
            // Act
            Result result = await IntegrationTestsFixture.SendAsync(command);
            
            // Assert
            result.IsError(out _).Should().BeFalse();
            
            ShoppingCart shoppingCartFromDb = await _shoppingCartHelper.GetShoppingCartAsync(command.ShoppingCartId);
            ShoppingCartItem shoppingCartItemFromDb = shoppingCartFromDb.Items
                .SingleOrDefault(sci => sci.Id == command.ShoppingCartItem.ProductId);
            shoppingCartItemFromDb.Should().NotBeNull();
        }
        
        [Fact]
        public async Task Remove_a_product_from_the_shopping_cart()
        {
            // Arrange
            ShoppingCart shoppingCart = await _shoppingCartHelper.PrepareShoppingCartForCheckoutAsync();
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
            
            ShoppingCart shoppingCartFromDb = await _shoppingCartHelper.GetShoppingCartAsync(command.ShoppingCartId);
            ShoppingCartItem shoppingCartItemFromDb = shoppingCartFromDb.Items
                .SingleOrDefault(sci => sci.Id == command.ProductId);
            shoppingCartItemFromDb.Should().BeNull();
        }
        
        [Fact]
        public async Task Set_customer_contact_information()
        {
            // Arrange
            ShoppingCart shoppingCart = await _shoppingCartHelper.PrepareShoppingCartForCheckoutAsync();

            FullName fullName = _autoFixture.Create<FullName>();
            GenderType gender = _autoFixture.Create<GenderType>();
            EmailAddress emailAddress = _autoFixture.Create<EmailAddress>();
            PhoneNumber phoneNumber = _autoFixture.Create<PhoneNumber>();
        
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

            ShoppingCart shoppingCartFromDb = await _shoppingCartHelper.GetShoppingCartAsync(command.ShoppingCartId);
            shoppingCartFromDb.Customer.FullName.Should().Be(fullName);
            shoppingCartFromDb.Customer.Gender.Should().Be(gender);
            shoppingCartFromDb.Customer.EmailAddress.Should().Be(emailAddress);
            shoppingCartFromDb.Customer.PhoneNumber.Should().Be(phoneNumber);
        }
        
        [Fact]
        public async Task Set_customer_delivery_address()
        {
            // Arrange
            ShoppingCart shoppingCart = await _shoppingCartHelper.PrepareShoppingCartForCheckoutAsync();

            Address deliveryAddress = _autoFixture.Create<Address>();
        
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
            
            ShoppingCart shoppingCartFromDb = await _shoppingCartHelper.GetShoppingCartAsync(command.ShoppingCartId);
            shoppingCartFromDb.Customer.DeliveryAddress.Should().Be(deliveryAddress);
        }
    }
}