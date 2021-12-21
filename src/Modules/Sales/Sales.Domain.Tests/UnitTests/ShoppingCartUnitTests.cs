using Xunit;
using System.Linq;
using FluentAssertions;

using VShop.SharedKernel.Infrastructure;
using VShop.SharedKernel.Infrastructure.Services;
using VShop.SharedKernel.Infrastructure.Services.Contracts;
using VShop.SharedKernel.Domain.ValueObjects;
using VShop.Modules.Sales.Tests.Customizations;
using VShop.Modules.Sales.Domain.Models.ShoppingCart;

namespace VShop.Modules.Sales.Domain.Tests.UnitTests
{
    public class ShoppingCartUnitTests
    {
        [Theory]
        [CustomizedAutoData]
        public void Product_insert_fails_when_shopping_cart_is_closed_for_updates
        (
            ShoppingCart sut,
            EntityId orderId,
            EntityId productId,
            ProductQuantity productQuantity,
            Price productPrice
        )
        {
            // Arrange
            IClockService clockService = new ClockService();

            sut.RequestCheckout(orderId, clockService.Now); // Checkout will prevent further updates

            // Act
            Result result = sut.AddProduct(productId, productQuantity, productPrice);
            
            // Assert
            result.IsError.Should().BeTrue();
        }

        [Theory]
        [CustomizedAutoData]
        public void Product_quantity_increment_fails_when_adding_product_with_different_price
        (
            EntityId shoppingCartId,
            EntityId customerId,
            Discount customerDiscount,
            EntityId productId,
            ProductQuantity productQuantity,
            Price productPrice
        )
        {
            // Arrange
            ShoppingCart sut = new();

            sut.Create(shoppingCartId, customerId, customerDiscount);
            sut.AddProduct(productId, productQuantity, productPrice);

            // Act
            Price newProductPrice = Price.Create(productPrice.Value + 1).Value;
            Result result = sut.AddProduct(productId, productQuantity, newProductPrice);
            
            // Assert
            result.IsError.Should().BeTrue();
        }
        
        [Theory]
        [CustomizedAutoData]
        public void Delivery_cost_is_applied_when_adding_product_and_not_enough_purchase_amount
        (
            EntityId shoppingCartId,
            EntityId customerId,
            EntityId productId
        )
        {
            // Arrange
            ShoppingCart sut = new();

            sut.Create(shoppingCartId, customerId, Discount.Create(0).Value);
            
            ProductQuantity productQuantity = ProductQuantity.Create(1).Value;
            Price productPrice = Price.Create(ShoppingCart.Settings.MinShoppingCartAmountForFreeDelivery - 1).Value;

            // Act
            Result result = sut.AddProduct(productId, productQuantity, productPrice);
            
            // Assert
            result.IsError.Should().BeFalse();
            sut.DeliveryCost.Value.Should().Be(ShoppingCart.Settings.DefaultDeliveryCost);
        }
        
        [Theory]
        [CustomizedAutoData]
        public void Delivery_cost_is_zero_when_adding_product_and_enough_purchase_amount
        (
            EntityId shoppingCartId,
            EntityId customerId,
            EntityId productId
        )
        {
            // Arrange
            ShoppingCart sut = new();

            sut.Create(shoppingCartId, customerId, Discount.Create(0).Value);
            
            ProductQuantity productQuantity = ProductQuantity.Create(1).Value;
            Price productPrice = Price.Create(ShoppingCart.Settings.MinShoppingCartAmountForFreeDelivery).Value;

            // Act
            Result result = sut.AddProduct(productId, productQuantity, productPrice);
            
            // Assert
            result.IsError.Should().BeFalse();
            sut.DeliveryCost.Value.Should().Be(0m);
        }
        
        [Theory]
        [CustomizedAutoData]
        public void Product_removal_fails_when_shopping_cart_is_closed_for_updates(ShoppingCart sut, EntityId orderId)
        {
            // Arrange
            IClockService clockService = new ClockService();

            sut.RequestCheckout(orderId, clockService.Now);
            
            ShoppingCartItem shoppingCartItem = sut.Items.First();
            
            // Act
            Result result = sut.RemoveProduct(shoppingCartItem.Id, shoppingCartItem.Quantity);
            
            // Assert
            result.IsError.Should().BeTrue();
        }
        
        [Theory]
        [CustomizedAutoData]
        public void Product_removal_increases_delivery_cost_when_not_enough_purchase_amount
        (
            EntityId shoppingCartId,
            EntityId customerId,
            EntityId productId
        )
        {
            // Arrange
            ShoppingCart sut = new();

            sut.Create(shoppingCartId, customerId, Discount.Create(0).Value);
            
            ProductQuantity productQuantity = ProductQuantity.Create(2).Value;
            Price productPrice = Price.Create(ShoppingCart.Settings.MinShoppingCartAmountForFreeDelivery - 1).Value;
            
            sut.AddProduct(productId, productQuantity, productPrice);

            // Act
            Result result = sut.RemoveProduct(productId, ProductQuantity.Create(1).Value);
            
            // Assert
            result.IsError.Should().BeFalse();
            sut.DeliveryCost.Value.Should().Be(ShoppingCart.Settings.DefaultDeliveryCost);
        }
        
        [Theory]
        [CustomizedAutoData]
        public void Delete_fails_for_already_deleted_shopping_cart(ShoppingCart sut)
        {
            // Arrange
            sut.RequestDelete();

            // Act
            Result result = sut.RequestDelete();
            
            // Assert
            result.IsError.Should().BeTrue();
        }

        [Theory]
        [CustomizedAutoData]
        public void Checkout_fails_when_shopping_cart_is_empty
        (
            EntityId shoppingCartId,
            EntityId customerId,
            Discount customerDiscount,
            EntityId orderId
        )
        {
            // Arrange
            IClockService clockService = new ClockService();
            
            ShoppingCart sut = new();
            sut.Create(shoppingCartId, customerId, customerDiscount);
            
            // Act
            Result result = sut.RequestCheckout(orderId, clockService.Now);
            
            // Assert
            result.IsError.Should().BeTrue();
        }
        
        [Theory]
        [CustomizedAutoData]
        public void Checkout_fails_when_not_enough_amount
        (
            EntityId shoppingCartId,
            EntityId customerId,
            Discount customerDiscount,
            EntityId productId,
            EntityId orderId
        )
        {
            // Arrange
            IClockService clockService = new ClockService();
            
            ShoppingCart sut = new();
            sut.Create(shoppingCartId, customerId, customerDiscount);
            
            ProductQuantity productQuantity = ProductQuantity.Create(1).Value;
            Price productPrice = Price.Create(ShoppingCart.Settings.MinShoppingCartAmountForCheckout - 1).Value;
            
            sut.AddProduct(productId, productQuantity, productPrice);
            
            // Act
            Result result = sut.RequestCheckout(orderId, clockService.Now);
            
            // Assert
            result.IsError.Should().BeTrue();
        }
        
        [Theory]
        [CustomizedAutoData]
        public void Checkout_fails_when_not_in_awaiting_confirmation_status
        (
            EntityId shoppingCartId,
            EntityId customerId,
            Discount customerDiscount,
            EntityId productId,
            EntityId orderId
        )
        {
            // Arrange
            IClockService clockService = new ClockService();
            
            ShoppingCart sut = new();
            sut.Create(shoppingCartId, customerId, customerDiscount);
            
            ProductQuantity productQuantity = ProductQuantity.Create(1).Value;
            Price productPrice = Price.Create(ShoppingCart.Settings.MinShoppingCartAmountForCheckout).Value;
            
            sut.AddProduct(productId, productQuantity, productPrice);
            
            // Act
            Result result = sut.RequestCheckout(orderId, clockService.Now);
            
            // Assert
            result.IsError.Should().BeTrue();
        }
    }
}