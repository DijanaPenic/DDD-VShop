using Xunit;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;

using VShop.SharedKernel.Domain.Enums;
using VShop.SharedKernel.Domain.ValueObjects;
using VShop.SharedKernel.Infrastructure;
using VShop.SharedKernel.Infrastructure.Extensions;
using VShop.SharedKernel.EventSourcing.Repositories.Contracts;
using VShop.Modules.Sales.Domain.Models.ShoppingCart;
using VShop.Modules.Sales.API.Application.Commands;
using VShop.Modules.Sales.API.Application.Commands.Shared;

namespace VShop.Modules.Sales.Tests.IntegrationTests
{
    [CollectionDefinition("Shopping Cart Integration Tests", DisableParallelization = true)]
    public class ShoppingCartIntegrationTests : IntegrationTestsBase, IClassFixture<AppFixture>
    {
        private readonly AppFixture _appFixture;
        private readonly Fixture _autoFixture;

        public ShoppingCartIntegrationTests(AppFixture appFixture)
        {
            _appFixture = appFixture;
            _autoFixture = appFixture.AutoFixture;
        }

        [Fact]
        public async Task Crete_a_new_shopping_cart()
        {
            // Arrange
            IAggregateRepository<ShoppingCart, EntityId> shoppingCartRepository = GetService<IAggregateRepository<ShoppingCart, EntityId>>();
            CreateShoppingCartCommandHandler sut = new(shoppingCartRepository);
            
            CreateShoppingCartCommand command = new()
            {
                ShoppingCartId = _autoFixture.Create<Guid>(),
                CustomerId = _autoFixture.Create<Guid>(),
                CustomerDiscount = _autoFixture.CreateInt(0, 100),
                ShoppingCartItems = _autoFixture.Create<ShoppingCartItemCommandDto[]>()
            };

            // Act
            Result<ShoppingCart> result = await sut.Handle(command, CancellationToken.None);
            
            // Assert
            result.IsError(out _).Should().BeFalse();
            
            ShoppingCart shoppingCartFromDb = await shoppingCartRepository.LoadAsync(EntityId.Create(command.ShoppingCartId));
            shoppingCartFromDb.Should().NotBeNull();
        }
        
        [Fact]
        public async Task Add_a_product_to_the_shopping_cart()
        {
            // Arrange
            IAggregateRepository<ShoppingCart, EntityId> shoppingCartRepository = GetService<IAggregateRepository<ShoppingCart, EntityId>>();
            AddShoppingCartProductCommandHandler sut = new(shoppingCartRepository);
            
            ShoppingCart shoppingCart = _appFixture.GetShoppingCartForCheckoutFixture();
            await shoppingCartRepository.SaveAsync(shoppingCart, CancellationToken.None);
            
            AddShoppingCartProductCommand command = new()
            {
                ShoppingCartId = shoppingCart.Id,
                ShoppingCartItem = _autoFixture.Create<ShoppingCartItemCommandDto>()
            };
            
            // Act
            Result result = await sut.Handle(command, CancellationToken.None);
            
            // Assert
            result.IsError(out _).Should().BeFalse();
            
            ShoppingCart shoppingCartFromDb = await shoppingCartRepository.LoadAsync(EntityId.Create(command.ShoppingCartId));
            
            ShoppingCartItem shoppingCartItemFromDb = shoppingCartFromDb.Items
                .SingleOrDefault(sci => sci.Id == command.ShoppingCartItem.ProductId);
            shoppingCartItemFromDb.Should().NotBeNull();
        }
        
        [Fact]
        public async Task Remove_a_product_from_the_shopping_cart()
        {
            // Arrange
            IAggregateRepository<ShoppingCart, EntityId> shoppingCartRepository = GetService<IAggregateRepository<ShoppingCart, EntityId>>();
            RemoveShoppingCartProductCommandHandler sut = new(shoppingCartRepository);

            ShoppingCart shoppingCart = _appFixture.GetShoppingCartForCheckoutFixture();
            await shoppingCartRepository.SaveAsync(shoppingCart, CancellationToken.None);
            
            ShoppingCartItem shoppingCartItem = shoppingCart.Items.First();
            RemoveShoppingCartProductCommand command = new()
            {
                ShoppingCartId = shoppingCart.Id,
                ProductId = shoppingCartItem.Id,
                Quantity = shoppingCartItem.Quantity
            };
            
            // Act
            Result result = await sut.Handle(command, CancellationToken.None);
            
            // Assert
            result.IsError(out _).Should().BeFalse();
            
            ShoppingCart shoppingCartFromDb = await shoppingCartRepository.LoadAsync(EntityId.Create(command.ShoppingCartId));
            
            ShoppingCartItem shoppingCartItemFromDb = shoppingCartFromDb.Items
                .SingleOrDefault(sci => sci.Id == command.ProductId);
            shoppingCartItemFromDb.Should().BeNull();
        }

        [Fact]
        public async Task Set_customer_contact_information()
        {
            // Arrange
            IAggregateRepository<ShoppingCart, EntityId> shoppingCartRepository = GetService<IAggregateRepository<ShoppingCart, EntityId>>();
            SetContactInformationCommandHandler sut = new(shoppingCartRepository);

            ShoppingCart shoppingCart = _appFixture.GetShoppingCartForCheckoutFixture();
            await shoppingCartRepository.SaveAsync(shoppingCart, CancellationToken.None);
            
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
            Result result = await sut.Handle(command, CancellationToken.None);
            
            // Assert
            result.IsError(out _).Should().BeFalse();
            
            ShoppingCart shoppingCartFromDb = await shoppingCartRepository.LoadAsync(EntityId.Create(command.ShoppingCartId));
            shoppingCartFromDb.Customer.FullName.Should().Be(fullName);
            shoppingCartFromDb.Customer.Gender.Should().Be(gender);
            shoppingCartFromDb.Customer.EmailAddress.Should().Be(emailAddress);
            shoppingCartFromDb.Customer.PhoneNumber.Should().Be(phoneNumber);
        }
        
        [Fact]
        public async Task Set_customer_delivery_address()
        {
            // Arrange
            IAggregateRepository<ShoppingCart, EntityId> shoppingCartRepository = GetService<IAggregateRepository<ShoppingCart, EntityId>>();
            SetDeliveryAddressCommandHandler sut = new(shoppingCartRepository);

            ShoppingCart shoppingCart = _appFixture.GetShoppingCartForCheckoutFixture();
            await shoppingCartRepository.SaveAsync(shoppingCart, CancellationToken.None);
            
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
            Result result = await sut.Handle(command, CancellationToken.None);
            
            // Assert
            result.IsError(out _).Should().BeFalse();
            
            ShoppingCart shoppingCartFromDb = await shoppingCartRepository.LoadAsync(EntityId.Create(command.ShoppingCartId));
            shoppingCartFromDb.Customer.DeliveryAddress.Should().Be(deliveryAddress);
        }
    }
}