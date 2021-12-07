using Xunit;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using AutoFixture;
using FluentAssertions;

using VShop.SharedKernel.Domain.Enums;
using VShop.SharedKernel.Domain.ValueObjects;
using VShop.SharedKernel.Infrastructure;
using VShop.SharedKernel.Infrastructure.Extensions;
using VShop.SharedKernel.EventSourcing.Repositories;
using VShop.Modules.Sales.Domain.Enums;
using VShop.Modules.Sales.Domain.Models.Ordering;
using VShop.Modules.Sales.Domain.Models.ShoppingCart;
using VShop.Modules.Sales.API.Application.Commands;
using VShop.Modules.Sales.API.Application.Commands.Shared;

namespace VShop.Modules.Sales.Testing
{
    [CollectionDefinition("Shopping Cart Integration Tests", DisableParallelization = true)]
    public class ShoppingCartIntegrationTests : IntegrationTests
    {
        [Fact]
        public async Task Crete_a_new_shopping_cart()
        {
            // Arrange
            EventStoreAggregateRepository<ShoppingCart, EntityId> shoppingCartRepository = Container
                .Resolve<EventStoreAggregateRepository<ShoppingCart, EntityId>>();
            CreateShoppingCartCommandHandler sut = new(shoppingCartRepository);
            
            CreateShoppingCartCommand command = new()
            {
                ShoppingCartId = Fixture.Create<Guid>(),
                CustomerId = Fixture.Create<Guid>(),
                CustomerDiscount = Fixture.CreateInt(0, 100),
                ShoppingCartItems = Fixture.Create<ShoppingCartItemCommandDto[]>()
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
            EventStoreAggregateRepository<ShoppingCart, EntityId> shoppingCartRepository = Container
                .Resolve<EventStoreAggregateRepository<ShoppingCart, EntityId>>();
            AddShoppingCartProductCommandHandler sut = new(shoppingCartRepository);
            
            ShoppingCart shoppingCart = await CreateShoppingCartInDatabaseAsync();
            AddShoppingCartProductCommand command = new()
            {
                ShoppingCartId = shoppingCart.Id,
                ShoppingCartItem = Fixture.Create<ShoppingCartItemCommandDto>()
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
            EventStoreAggregateRepository<ShoppingCart, EntityId> shoppingCartRepository = Container
                .Resolve<EventStoreAggregateRepository<ShoppingCart, EntityId>>();
            RemoveShoppingCartProductCommandHandler sut = new(shoppingCartRepository);

            ShoppingCart shoppingCart = await CreateShoppingCartInDatabaseAsync();
            
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
            EventStoreAggregateRepository<ShoppingCart, EntityId> shoppingCartRepository = Container
                .Resolve<EventStoreAggregateRepository<ShoppingCart, EntityId>>();
            SetContactInformationCommandHandler sut = new(shoppingCartRepository);

            ShoppingCart shoppingCart = await CreateShoppingCartInDatabaseAsync();
            
            FullName fullName = Fixture.Create<FullName>();
            GenderType gender = Fixture.Create<GenderType>();
            EmailAddress emailAddress = Fixture.Create<EmailAddress>();
            PhoneNumber phoneNumber = Fixture.Create<PhoneNumber>();

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
            EventStoreAggregateRepository<ShoppingCart, EntityId> shoppingCartRepository = Container
                .Resolve<EventStoreAggregateRepository<ShoppingCart, EntityId>>();
            SetDeliveryAddressCommandHandler sut = new(shoppingCartRepository);

            ShoppingCart shoppingCart = await CreateShoppingCartInDatabaseAsync();
            Address deliveryAddress = Fixture.Create<Address>();

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
        
        [Fact]
        public async Task Checkout_the_shopping_cart()
        {
            // Arrange
            EventStoreAggregateRepository<ShoppingCart, EntityId> shoppingCartRepository = Container
                .Resolve<EventStoreAggregateRepository<ShoppingCart, EntityId>>();
            EventStoreAggregateRepository<Order, EntityId> orderRepository = Container
                .Resolve<EventStoreAggregateRepository<Order, EntityId>>();
            CheckoutShoppingCartCommandHandler sut = new(shoppingCartRepository);

            ShoppingCart shoppingCart = await CreateShoppingCartInDatabaseAsync();

            CheckoutShoppingCartCommand command = new(shoppingCart.Id);
            
            // Act
            Result<CheckoutOrder> result = await sut.Handle(command, CancellationToken.None);
            
            // Assert
            result.IsError(out _).Should().BeFalse();
            
            ShoppingCart shoppingCartFromDb = await shoppingCartRepository.LoadAsync(EntityId.Create(command.ShoppingCartId));
            shoppingCartFromDb.Status.Should().Be(ShoppingCartStatus.Closed); // The shopping cart should have been deleted
            
            Order orderFromDb = await orderRepository.LoadAsync(EntityId.Create(result.GetData().OrderId));
            orderFromDb.Should().NotBeNull(); // The order should have been created
        }

        private async Task<ShoppingCart> CreateShoppingCartInDatabaseAsync()
        {
            ShoppingCart shoppingCart = new();

            shoppingCart.Create
            (
                Fixture.Create<EntityId>(),
                Fixture.Create<EntityId>(),
                Fixture.CreateInt(0, 100)
            );
            
            while(!shoppingCart.HasMinAmountForCheckout)
            {
                ShoppingCartItemCommandDto shoppingCartItem = Fixture.Create<ShoppingCartItemCommandDto>();
                shoppingCart.AddProduct
                (
                    EntityId.Create(shoppingCartItem.ProductId),
                    ProductQuantity.Create(shoppingCartItem.Quantity),
                    Price.Create(shoppingCartItem.UnitPrice)
                );
            };
            
            shoppingCart.Customer.SetDeliveryAddress(Fixture.Create<Address>());
            shoppingCart.Customer.SetContactInformation
            (
                Fixture.Create<FullName>(),
                Fixture.Create<EmailAddress>(),
                Fixture.Create<PhoneNumber>(),
                Fixture.Create<GenderType>()
            );

            EventStoreAggregateRepository<ShoppingCart, EntityId> shoppingCartRepository = Container
                .Resolve<EventStoreAggregateRepository<ShoppingCart, EntityId>>();
            await shoppingCartRepository.SaveAsync(shoppingCart, CancellationToken.None);

            return shoppingCart;
        }
    }
}