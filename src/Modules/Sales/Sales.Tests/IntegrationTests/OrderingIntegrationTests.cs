using Xunit;
using AutoFixture;
using FluentAssertions;
using System.Threading;
using System.Threading.Tasks;

using VShop.SharedKernel.Infrastructure;
using VShop.SharedKernel.Domain.ValueObjects;
using VShop.SharedKernel.Infrastructure.Services.Contracts;
using VShop.SharedKernel.EventSourcing.Repositories.Contracts;
using VShop.Modules.Sales.Domain.Enums;
using VShop.Modules.Sales.Domain.Models.Ordering;
using VShop.Modules.Sales.Domain.Models.ShoppingCart;
using VShop.Modules.Sales.API.Application.Commands;

namespace VShop.Modules.Sales.Tests.IntegrationTests
{
    [CollectionDefinition("Ordering Integration Tests", DisableParallelization = true)]
    public class OrderingIntegrationTests : IntegrationTestsBase, IClassFixture<AppFixture>
    {
        private readonly AppFixture _appFixture;
        private readonly Fixture _autoFixture;

        public OrderingIntegrationTests(AppFixture appFixture)
        {
            _appFixture = appFixture;
            _autoFixture = appFixture.AutoFixture;
        }
        
        [Fact]
        public async Task Checkout_the_shopping_cart()
        {
            // Arrange
            IAggregateRepository<ShoppingCart, EntityId> shoppingCartRepository = GetService<IAggregateRepository<ShoppingCart, EntityId>>();
            IAggregateRepository<Order, EntityId> orderRepository = GetService<IAggregateRepository<Order, EntityId>>();
            IClockService clockService = GetService<IClockService>();
            
            CheckoutShoppingCartCommandHandler sut = new(clockService, shoppingCartRepository);

            ShoppingCart shoppingCart = _appFixture.GetShoppingCartForCheckoutFixture();
            await shoppingCartRepository.SaveAsync(shoppingCart, CancellationToken.None);

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
    }
}