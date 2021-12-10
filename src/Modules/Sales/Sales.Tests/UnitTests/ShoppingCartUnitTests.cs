using Xunit;
using System.Linq;
using AutoFixture;
using FluentAssertions;

using VShop.SharedKernel.Domain.ValueObjects;
using VShop.SharedKernel.Infrastructure;
using VShop.SharedKernel.Infrastructure.Services;
using VShop.SharedKernel.Infrastructure.Extensions;
using VShop.Modules.Sales.Domain.Models.ShoppingCart;

namespace VShop.Modules.Sales.Tests.UnitTests
{
    public class ShoppingCartUnitTests : UnitTestsBase, IClassFixture<AppFixture>
    {
        private readonly AppFixture _appFixture;
        private readonly Fixture _autoFixture;

        public ShoppingCartUnitTests(AppFixture appFixture)
        {
            _appFixture = appFixture;
            _autoFixture = appFixture.AutoFixture;
        }
        
        [Fact]
        public void Product_insert_fails_when_shopping_cart_is_closed_for_updates()
        {
            // Arrange
            ShoppingCart sut = _appFixture.GetShoppingCartForCheckoutFixture();
            
            sut.RequestCheckout(new ClockService(), _autoFixture.Create<EntityId>()); // Checkout will prevent further updates
            
            EntityId productId = _autoFixture.Create<EntityId>();
            ProductQuantity productQuantity = _autoFixture.Create<ProductQuantity>();
            Price productPrice = _autoFixture.Create<Price>();
            
            // Act
            Result result = sut.AddProduct(productId, productQuantity, productPrice);
            
            // Assert
            result.IsError(out _).Should().BeTrue();
        }

        [Fact]
        public void Product_quantity_increment_fails_when_adding_product_with_different_price()
        {
            // Arrange
            ShoppingCart sut = new();

            sut.Create
            (
                _autoFixture.Create<EntityId>(),
                _autoFixture.Create<EntityId>(),
                _autoFixture.CreateInt(0, 100)
            );

            EntityId productId = _autoFixture.Create<EntityId>();
            ProductQuantity productQuantity = _autoFixture.Create<ProductQuantity>();
            Price productPrice = _autoFixture.Create<Price>();
            
            sut.AddProduct(productId, productQuantity, productPrice);

            // Act
            Price newProductPrice = Price.Create(productPrice.Value + 1);
            Result result = sut.AddProduct(productId, productQuantity, newProductPrice);
            
            // Assert
            result.IsError(out _).Should().BeTrue();
        }
        
        [Fact]
        public void Delivery_cost_is_applied_when_adding_product_and_not_enough_purchase_amount()
        {
            // Arrange
            ShoppingCart sut = new();

            sut.Create
            (
                _autoFixture.Create<EntityId>(),
                _autoFixture.Create<EntityId>(),
                0
            );

            EntityId productId = _autoFixture.Create<EntityId>();
            ProductQuantity productQuantity = ProductQuantity.Create(1);
            Price productPrice = Price.Create(ShoppingCart.Settings.MinShoppingCartAmountForFreeDelivery - 1);

            // Act
            Result result = sut.AddProduct(productId, productQuantity, productPrice);
            
            // Assert
            result.IsError(out _).Should().BeFalse();
            sut.DeliveryCost.Value.Should().Be(ShoppingCart.Settings.DefaultDeliveryCost);
        }
        
        [Fact]
        public void Delivery_cost_is_zero_when_adding_product_and_enough_purchase_amount()
        {
            // Arrange
            ShoppingCart sut = new();

            sut.Create
            (
                _autoFixture.Create<EntityId>(),
                _autoFixture.Create<EntityId>(),
                0
            );

            EntityId productId = _autoFixture.Create<EntityId>();
            ProductQuantity productQuantity = ProductQuantity.Create(1);
            Price productPrice = Price.Create(ShoppingCart.Settings.MinShoppingCartAmountForFreeDelivery);

            // Act
            Result result = sut.AddProduct(productId, productQuantity, productPrice);
            
            // Assert
            result.IsError(out _).Should().BeFalse();
            sut.DeliveryCost.Value.Should().Be(0m);
        }
        
        [Fact]
        public void Product_removal_fails_when_shopping_cart_is_closed_for_updates()
        {
            // Arrange
            ShoppingCart sut = _appFixture.GetShoppingCartForCheckoutFixture();
            
            sut.RequestCheckout(new ClockService(),_autoFixture.Create<EntityId>());
            
            ShoppingCartItem shoppingCartItem = sut.Items.First();
            
            // Act
            Result result = sut.RemoveProduct(shoppingCartItem.Id, shoppingCartItem.Quantity);
            
            // Assert
            result.IsError(out _).Should().BeTrue();
        }
        
        [Fact]
        public void Product_removal_increases_delivery_cost_when_not_enough_purchase_amount()
        {
            // Arrange
            ShoppingCart sut = new();

            sut.Create
            (
                _autoFixture.Create<EntityId>(),
                _autoFixture.Create<EntityId>(),
                0
            );

            EntityId productId = _autoFixture.Create<EntityId>();
            ProductQuantity productQuantity = ProductQuantity.Create(2);
            Price productPrice = Price.Create(ShoppingCart.Settings.MinShoppingCartAmountForFreeDelivery - 1);
            
            sut.AddProduct(productId, productQuantity, productPrice);

            // Act
            Result result = sut.RemoveProduct(productId, ProductQuantity.Create(1));
            
            // Assert
            result.IsError(out _).Should().BeFalse();
            sut.DeliveryCost.Value.Should().Be(ShoppingCart.Settings.DefaultDeliveryCost);
        }
        
        [Fact]
        public void Delete_fails_for_already_deleted_shopping_cart()
        {
            // Arrange
            ShoppingCart sut = _appFixture.GetShoppingCartForCheckoutFixture();

            sut.RequestDelete();

            // Act
            Result result = sut.RequestDelete();
            
            // Assert
            result.IsError(out _).Should().BeTrue();
        }

        [Fact]
        public void Checkout_fails_when_shopping_cart_is_empty()
        {
            // Arrange
            ShoppingCart sut = new();

            sut.Create
            (
                _autoFixture.Create<EntityId>(),
                _autoFixture.Create<EntityId>(),
                _autoFixture.CreateInt(0, 100)
            );
            
            // Act
            Result result = sut.RequestCheckout(new ClockService(), _autoFixture.Create<EntityId>());
            
            // Assert
            result.IsError(out _).Should().BeTrue();
        }
        
        [Fact]
        public void Checkout_fails_when_not_enough_amount()
        {
            // Arrange
            ShoppingCart sut = new();

            sut.Create
            (
                _autoFixture.Create<EntityId>(),
                _autoFixture.Create<EntityId>(),
                _autoFixture.CreateInt(0, 100)
            );
            
            EntityId productId = _autoFixture.Create<EntityId>();
            ProductQuantity productQuantity = ProductQuantity.Create(1);
            Price productPrice = Price.Create(ShoppingCart.Settings.MinShoppingCartAmountForCheckout - 1);
            
            sut.AddProduct(productId, productQuantity, productPrice);
            
            // Act
            Result result = sut.RequestCheckout(new ClockService(), _autoFixture.Create<EntityId>());
            
            // Assert
            result.IsError(out _).Should().BeTrue();
        }
        
        [Fact]
        public void Checkout_fails_when_not_in_awaiting_confirmation_status()
        {
            // Arrange
            ShoppingCart sut = new();

            sut.Create
            (
                _autoFixture.Create<EntityId>(),
                _autoFixture.Create<EntityId>(),
                _autoFixture.CreateInt(0, 100)
            );
            
            EntityId productId = _autoFixture.Create<EntityId>();
            ProductQuantity productQuantity = ProductQuantity.Create(1);
            Price productPrice = Price.Create(ShoppingCart.Settings.MinShoppingCartAmountForCheckout);
            
            sut.AddProduct(productId, productQuantity, productPrice);
            
            // Act
            Result result = sut.RequestCheckout(new ClockService(), _autoFixture.Create<EntityId>());
            
            // Assert
            result.IsError(out _).Should().BeTrue();
        }
    }
}