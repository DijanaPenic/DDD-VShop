using Xunit;
using System.Linq;
using AutoFixture;
using FluentAssertions;

using VShop.SharedKernel.Domain.Enums;
using VShop.SharedKernel.Domain.ValueObjects;
using VShop.SharedKernel.Infrastructure;
using VShop.SharedKernel.Infrastructure.Extensions;
using VShop.Modules.Sales.Domain.Models.ShoppingCart;
using VShop.Modules.Sales.API.Application.Commands.Shared;

namespace VShop.Modules.Sales.Testing
{
    public class ShoppingCartUnitTests : UnitTests
    {
        [Fact]
        public void Product_insert_fails_when_shopping_cart_is_closed_for_updates()
        {
            // Arrange
            ShoppingCart sut = GetShoppingCartForCheckoutFixture();
            
            sut.RequestCheckout(Fixture.Create<EntityId>());
            
            EntityId productId = Fixture.Create<EntityId>();
            ProductQuantity productQuantity = Fixture.Create<ProductQuantity>();
            Price productPrice = Fixture.Create<Price>();
            
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
                Fixture.Create<EntityId>(),
                Fixture.Create<EntityId>(),
                Fixture.CreateInt(0, 100)
            );

            EntityId productId = Fixture.Create<EntityId>();
            ProductQuantity productQuantity = Fixture.Create<ProductQuantity>();
            Price productPrice = Fixture.Create<Price>();
            
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
                Fixture.Create<EntityId>(),
                Fixture.Create<EntityId>(),
                0
            );

            EntityId productId = Fixture.Create<EntityId>();
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
                Fixture.Create<EntityId>(),
                Fixture.Create<EntityId>(),
                0
            );

            EntityId productId = Fixture.Create<EntityId>();
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
            ShoppingCart sut = GetShoppingCartForCheckoutFixture();
            
            sut.RequestCheckout(Fixture.Create<EntityId>());
            
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
                Fixture.Create<EntityId>(),
                Fixture.Create<EntityId>(),
                0
            );

            EntityId productId = Fixture.Create<EntityId>();
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
            ShoppingCart sut = GetShoppingCartForCheckoutFixture();

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
                Fixture.Create<EntityId>(),
                Fixture.Create<EntityId>(),
                Fixture.CreateInt(0, 100)
            );
            
            // Act
            Result result = sut.RequestCheckout(Fixture.Create<EntityId>());
            
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
                Fixture.Create<EntityId>(),
                Fixture.Create<EntityId>(),
                Fixture.CreateInt(0, 100)
            );
            
            EntityId productId = Fixture.Create<EntityId>();
            ProductQuantity productQuantity = ProductQuantity.Create(1);
            Price productPrice = Price.Create(ShoppingCart.Settings.MinShoppingCartAmountForCheckout - 1);
            
            sut.AddProduct(productId, productQuantity, productPrice);
            
            // Act
            Result result = sut.RequestCheckout(Fixture.Create<EntityId>());
            
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
                Fixture.Create<EntityId>(),
                Fixture.Create<EntityId>(),
                Fixture.CreateInt(0, 100)
            );
            
            EntityId productId = Fixture.Create<EntityId>();
            ProductQuantity productQuantity = ProductQuantity.Create(1);
            Price productPrice = Price.Create(ShoppingCart.Settings.MinShoppingCartAmountForCheckout);
            
            sut.AddProduct(productId, productQuantity, productPrice);
            
            // Act
            Result result = sut.RequestCheckout(Fixture.Create<EntityId>());
            
            // Assert
            result.IsError(out _).Should().BeTrue();
        }

        private ShoppingCart GetShoppingCartForCheckoutFixture()
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

            return shoppingCart;
        }
    }
}