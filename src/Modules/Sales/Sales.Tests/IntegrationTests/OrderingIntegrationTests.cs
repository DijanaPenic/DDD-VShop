using Xunit;
using Autofac;
using AutoFixture;
using FluentAssertions;
using System.Threading;
using System.Threading.Tasks;

using VShop.SharedKernel.Domain.Enums;
using VShop.SharedKernel.Domain.ValueObjects;
using VShop.SharedKernel.Infrastructure;
using VShop.SharedKernel.Infrastructure.Extensions;
using VShop.SharedKernel.EventSourcing.Repositories.Contracts;
using VShop.Modules.Sales.Domain.Enums;
using VShop.Modules.Sales.Domain.Models.Ordering;
using VShop.Modules.Sales.Domain.Models.ShoppingCart;
using VShop.Modules.Sales.API.Application.Commands;
using VShop.Modules.Sales.API.Application.Commands.Shared;

namespace VShop.Modules.Sales.Tests.IntegrationTests
{
    [CollectionDefinition("Ordering Integration Tests", DisableParallelization = true)]
    public class OrderingIntegrationTests : IntegrationTestsBase, IClassFixture<AppFixture>
    {
        private readonly Fixture _autoFixture;

        public OrderingIntegrationTests(AppFixture appFixture) => _autoFixture = appFixture.AutoFixture;
        
        [Fact]
        public async Task Checkout_the_shopping_cart()
        {
            // Arrange
            IAggregateRepository<ShoppingCart, EntityId> shoppingCartRepository = Container
                .Resolve<IAggregateRepository<ShoppingCart, EntityId>>();
            IAggregateRepository<Order, EntityId> orderRepository = Container
                .Resolve<IAggregateRepository<Order, EntityId>>();
            CheckoutShoppingCartCommandHandler sut = new(shoppingCartRepository);

            ShoppingCart shoppingCart = await CreateShoppingCartInDatabaseAsync();

            CheckoutShoppingCartCommand command = new(shoppingCart.Id);
            
            // Act
            Result<CheckoutOrder> result = await sut.Handle(command, CancellationToken.None);
            
            // Assert
            result.IsError(out _).Should().BeFalse();
            
            ShoppingCart shoppingCartFromDb = await shoppingCartRepository.LoadAsync(EntityId.Create(command.ShoppingCartId));
            shoppingCartFromDb.Status.Should().Be(ShoppingCartStatus.Closed); // The shopping cart should have been deleted.
            
            Order orderFromDb = await orderRepository.LoadAsync(EntityId.Create(result.GetData().OrderId));
            orderFromDb.Should().NotBeNull(); // The order should have been created.
        }
        
        [Fact]
        public void Test()
        {
        
        }

        // TODO - move this to some shared class
        private async Task<ShoppingCart> CreateShoppingCartInDatabaseAsync()
        {
            ShoppingCart shoppingCart = new();

            shoppingCart.Create
            (
                _autoFixture.Create<EntityId>(),
                _autoFixture.Create<EntityId>(),
                _autoFixture.CreateInt(0, 100)
            );
            
            while(!shoppingCart.HasMinAmountForCheckout)
            {
                ShoppingCartItemCommandDto shoppingCartItem = _autoFixture.Create<ShoppingCartItemCommandDto>();
                shoppingCart.AddProduct
                (
                    EntityId.Create(shoppingCartItem.ProductId),
                    ProductQuantity.Create(shoppingCartItem.Quantity),
                    Price.Create(shoppingCartItem.UnitPrice)
                );
            };
            
            shoppingCart.Customer.SetDeliveryAddress(_autoFixture.Create<Address>());
            shoppingCart.Customer.SetContactInformation
            (
                _autoFixture.Create<FullName>(),
                _autoFixture.Create<EmailAddress>(),
                _autoFixture.Create<PhoneNumber>(),
                _autoFixture.Create<GenderType>()
            );

            IAggregateRepository<ShoppingCart, EntityId> shoppingCartRepository = Container
                .Resolve<IAggregateRepository<ShoppingCart, EntityId>>();
            await shoppingCartRepository.SaveAsync(shoppingCart, CancellationToken.None);

            return shoppingCart;
        }
    }
}