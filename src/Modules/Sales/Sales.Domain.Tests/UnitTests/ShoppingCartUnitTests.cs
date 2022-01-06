using Xunit;
using System;
using System.Linq;
using FluentAssertions;

using VShop.SharedKernel.Domain.Enums;
using VShop.SharedKernel.Infrastructure;
using VShop.SharedKernel.Infrastructure.Services;
using VShop.SharedKernel.Infrastructure.Services.Contracts;
using VShop.SharedKernel.Domain.ValueObjects;
using VShop.Modules.Sales.Domain.Enums;
using VShop.Modules.Sales.Domain.Models.ShoppingCart;
using VShop.Modules.Sales.Tests.Customizations;

namespace VShop.Modules.Sales.Domain.Tests.UnitTests
{
    public class ShoppingCartUnitTests
    {
        [Theory]
        [CustomizedAutoData]
        public void Creates_a_new_shopping_cart
        (
            EntityId shoppingCartId,
            EntityId customerId,
            Discount customerDiscount,
            Guid causationId,
            Guid correlationId
        )
        {
            // Act
            Result<ShoppingCart> result = ShoppingCart.Create
            (
                shoppingCartId,
                customerId,
                customerDiscount,
                causationId,
                correlationId
            );
            
            // Assert
            result.IsError.Should().BeFalse();

            ShoppingCart shoppingCart = result.Data;
            shoppingCart.Should().NotBeNull();
            shoppingCart.Status.Should().Be(ShoppingCartStatus.New);
            shoppingCart.DeliveryCost.Value.Should().Be(ShoppingCart.Settings.DefaultDeliveryCost);
        }
        
        [Theory]
        [CustomizedAutoData]
        public void Product_quantity_increment_adds_a_new_product_to_the_shopping_cart
        (
            ShoppingCart sut,
            EntityId productId,
            ProductQuantity productQuantity,
            Price productPrice
        )
        {
            // Act
            Result result = sut.AddProductQuantity(productId, productQuantity, productPrice);
            
            // Assert
            result.IsError.Should().BeFalse();
            
            ShoppingCartItem shoppingCartItem = sut.Items
                .FirstOrDefault(p => Equals(p.Id, productId));
            shoppingCartItem.Should().NotBeNull();
        }
        
        [Theory]
        [CustomizedAutoData]
        public void Product_quantity_increment_increases_the_existing_product_quantity           
        (
            ShoppingCart sut,
            EntityId productId,
            Price productPrice
        )
        {
            // Arrange
            sut.AddProductQuantity(productId, ProductQuantity.Create(2).Data, productPrice);
            ProductQuantity addQuantity = ProductQuantity.Create(1).Data;

            // Act
            Result result = sut.AddProductQuantity(productId, addQuantity, productPrice);
            
            // Assert
            result.IsError.Should().BeFalse();
            
            ShoppingCartItem shoppingCartItem = sut.Items
                .FirstOrDefault(p => Equals(p.Id, productId));
            shoppingCartItem.Should().NotBeNull();
            shoppingCartItem!.Quantity.Value.Should().Be(3);
        }

        
        [Theory]
        [CustomizedAutoData]
        public void  Product_quantity_increment_fails_when_shopping_cart_is_closed_for_updates
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
            sut.Checkout(orderId, clockService.Now); // Checkout will prevent further updates

            // Act
            Result result = sut.AddProductQuantity(productId, productQuantity, productPrice);
            
            // Assert
            result.IsError.Should().BeTrue();
        }

        [Theory]
        [CustomizedAutoData]
        public void Product_quantity_increment_fails_when_price_mismatch
        (
            ShoppingCart sut,
            EntityId productId,
            ProductQuantity productQuantity,
            Price productPrice
        )
        {
            // Arrange
            sut.AddProductQuantity(productId, productQuantity, productPrice);
            Price newProductPrice = Price.Create(productPrice.Value + 1).Data;
            
            // Act
            Result result = sut.AddProductQuantity(productId, productQuantity, newProductPrice);
            
            // Assert
            result.IsError.Should().BeTrue();
        }
        
        [Theory]
        [CustomizedAutoData]
        public void Product_quantity_increment_fails_when_per_product_quantity_is_more_than_allowed
        (
            ShoppingCart sut,
            EntityId productId,
            Price productPrice
        )
        {
            // Arrange
            ProductQuantity productQuantity = ProductQuantity.Create(ShoppingCartItem.Settings.MaxQuantityPerProduct).Data;
            sut.AddProductQuantity(productId, productQuantity, productPrice);

            ProductQuantity newProductQuantity = ProductQuantity.Create(1).Data;
            
            // Act
            Result result = sut.AddProductQuantity(productId, newProductQuantity, productPrice);
            
            // Assert
            result.IsError.Should().BeTrue();
        }
        
        [Theory]
        [CustomizedAutoData]
        public void Delivery_cost_is_applied_when_adding_product_quantity_and_not_enough_purchase_amount
        (
            EntityId shoppingCartId,
            EntityId customerId,
            EntityId productId,
            Guid causationId,
            Guid correlationId
        )
        {
            // Arrange
            ShoppingCart sut = ShoppingCart
                .Create(shoppingCartId, customerId, Discount.Create(0).Data, causationId, correlationId).Data;

            ProductQuantity productQuantity = ProductQuantity.Create(1).Data;
            Price productPrice = Price.Create(ShoppingCart.Settings.MinShoppingCartAmountForFreeDelivery - 1).Data;

            // Act
            Result result = sut.AddProductQuantity(productId, productQuantity, productPrice);
            
            // Assert
            result.IsError.Should().BeFalse();
            sut.DeliveryCost.Value.Should().Be(ShoppingCart.Settings.DefaultDeliveryCost);
        }
        
        [Theory]
        [CustomizedAutoData]
        public void Delivery_cost_is_zero_when_adding_product_quantity_and_enough_purchase_amount
        (
            EntityId shoppingCartId,
            EntityId customerId,
            EntityId productId,
            Guid causationId,
            Guid correlationId
        )
        {
            // Arrange
            ShoppingCart sut = ShoppingCart
                .Create(shoppingCartId, customerId, Discount.Create(0).Data, causationId, correlationId).Data;

            ProductQuantity productQuantity = ProductQuantity.Create(1).Data;
            Price productPrice = Price.Create(ShoppingCart.Settings.MinShoppingCartAmountForFreeDelivery).Data;

            // Act
            Result result = sut.AddProductQuantity(productId, productQuantity, productPrice);
            
            // Assert
            result.IsError.Should().BeFalse();
            sut.DeliveryCost.Value.Should().Be(0m);
        }
        
        [Theory]
        [CustomizedAutoData]
        public void Product_quantity_removal_removes_product_from_the_shopping_cart(ShoppingCart sut)
        {
            // Arrange
            ShoppingCartItem shoppingCartItem = sut.Items[0];
            
            // Act
            Result result = sut.RemoveProductQuantity(shoppingCartItem.Id, shoppingCartItem.Quantity);
            
            // Assert
            result.IsError.Should().BeFalse();
            
            shoppingCartItem = sut.Items
                .FirstOrDefault(p => Equals(p.Id, shoppingCartItem.Id));
            shoppingCartItem.Should().BeNull();
        }
        
        [Theory]
        [CustomizedAutoData]
        public void Product_quantity_removal_decreases_the_existing_product_quantity           
        (
            ShoppingCart sut,
            EntityId productId,
            Price productPrice
        )
        {
            // Arrange
            sut.AddProductQuantity(productId, ProductQuantity.Create(2).Data, productPrice);
            ProductQuantity removeQuantity = ProductQuantity.Create(1).Data;

            // Act
            Result result = sut.RemoveProductQuantity(productId, removeQuantity);
            
            // Assert
            result.IsError.Should().BeFalse();
            
            ShoppingCartItem shoppingCartItem = sut.Items
                .FirstOrDefault(p => Equals(p.Id, productId));
            shoppingCartItem.Should().NotBeNull();
            shoppingCartItem!.Quantity.Value.Should().Be(1);
        }
        
        [Theory]
        [CustomizedAutoData]
        public void Product_quantity_removal_fails_when_shopping_cart_is_closed_for_updates(ShoppingCart sut, EntityId orderId)
        {
            // Arrange
            IClockService clockService = new ClockService();
            sut.Checkout(orderId, clockService.Now);
            
            ShoppingCartItem shoppingCartItem = sut.Items[0];
            
            // Act
            Result result = sut.RemoveProductQuantity(shoppingCartItem.Id, shoppingCartItem.Quantity);
            
            // Assert
            result.IsError.Should().BeTrue();
        }
        
        [Theory]
        [CustomizedAutoData]
        public void Product_quantity_removal_increases_delivery_cost_when_not_enough_purchase_amount
        (
            EntityId shoppingCartId,
            EntityId customerId,
            EntityId productId,
            Guid causationId,
            Guid correlationId
        )
        {
            // Arrange
            ShoppingCart sut = ShoppingCart
                .Create(shoppingCartId, customerId, Discount.Create(0).Data, causationId, correlationId).Data;

            ProductQuantity productQuantity = ProductQuantity.Create(2).Data;
            Price productPrice = Price.Create(ShoppingCart.Settings.MinShoppingCartAmountForFreeDelivery - 1).Data;
            
            sut.AddProductQuantity(productId, productQuantity, productPrice);

            // Act
            Result result = sut.RemoveProductQuantity(productId, ProductQuantity.Create(1).Data);
            
            // Assert
            result.IsError.Should().BeFalse();
            sut.DeliveryCost.Value.Should().Be(ShoppingCart.Settings.DefaultDeliveryCost);
        }
        
        [Theory]
        [CustomizedAutoData]
        public void Decreasing_the_existing_product_quantity_fails_when_too_high_removal_quantity
        (
            ShoppingCart shoppingCart
        )
        {
            // Arrange
            ShoppingCartItem sut = shoppingCart.Items[0];
            ProductQuantity productQuantity = ProductQuantity.Create(sut.Quantity + 1).Data;

            // Act
            Result result = sut.DecreaseQuantity(productQuantity);
            
            // Assert
            result.IsError.Should().BeTrue();
        }
        
        [Theory]
        [CustomizedAutoData]
        public void Deletes_the_shopping_cart(ShoppingCart sut)
        {
            // Act
            Result result = sut.Delete();
            
            // Assert
            result.IsError.Should().BeFalse();
            sut.Status.Should().Be(ShoppingCartStatus.Closed);
        }
        
        [Theory]
        [CustomizedAutoData]
        public void Shopping_cart_delete_fails_for_already_deleted_shopping_cart(ShoppingCart sut)
        {
            // Arrange
            sut.Delete();

            // Act
            Result result = sut.Delete();
            
            // Assert
            result.IsError.Should().BeTrue();
        }
        
        [Theory]
        [CustomizedAutoData]
        public void Sets_customer_contact_information
        (
            ShoppingCart shoppingCart,
            FullName fullName,
            EmailAddress emailAddress,
            PhoneNumber phoneNumber,
            GenderType genderType
        )
        {
            // Arrange
            ShoppingCartCustomer sut = shoppingCart.Customer;
            
            // Act
            Result result = sut.SetContactInformation(fullName, emailAddress, phoneNumber, genderType);
            
            // Assert
            result.IsError.Should().BeFalse();
        }
        
        [Theory]
        [CustomizedAutoData]
        public void Setting_customer_contact_information_fails_when_shopping_cart_is_closed_for_updates
        (
            ShoppingCart shoppingCart,
            EntityId orderId,
            FullName fullName,
            EmailAddress emailAddress,
            PhoneNumber phoneNumber,
            GenderType genderType
        )
        {
            // Arrange
            IClockService clockService = new ClockService();
            ShoppingCartCustomer sut = shoppingCart.Customer;

            shoppingCart.Checkout(orderId, clockService.Now); // Checkout will prevent further updates

            // Act
            Result result = sut.SetContactInformation(fullName, emailAddress, phoneNumber, genderType);
            
            // Assert
            result.IsError.Should().BeTrue();
        }
        
        [Theory]
        [CustomizedAutoData]
        public void Sets_customer_delivery_address
        (
            ShoppingCart shoppingCart,
            Address deliveryAddress
        )
        {
            // Arrange
            ShoppingCartCustomer sut = shoppingCart.Customer;
            
            // Act
            Result result = sut.SetDeliveryAddress(deliveryAddress);
            
            // Assert
            result.IsError.Should().BeFalse();
        }
        
        [Theory]
        [CustomizedAutoData]
        public void Setting_customer_delivery_address_fails_when_shopping_cart_is_closed_for_updates
        (
            ShoppingCart shoppingCart,
            EntityId orderId,
            Address deliveryAddress
        )
        {
            // Arrange
            IClockService clockService = new ClockService();
            ShoppingCartCustomer sut = shoppingCart.Customer;

            shoppingCart.Checkout(orderId, clockService.Now); // Checkout will prevent further updates

            // Act
            Result result = sut.SetDeliveryAddress(deliveryAddress);
            
            // Assert
            result.IsError.Should().BeTrue();
        }
        
        [Theory]
        [CustomizedAutoData]
        public void Checkouts_the_shopping_cart(ShoppingCart sut, EntityId orderId)
        {
            // Arrange
            IClockService clockService = new ClockService();

            // Act
            Result result = sut.Checkout(orderId, clockService.Now);
            
            // Assert
            result.IsError.Should().BeFalse();
            sut.Status.Should().Be(ShoppingCartStatus.PendingCheckout);
        }

        [Theory]
        [CustomizedAutoData]
        public void Shopping_cart_checkout_fails_when_shopping_cart_is_empty
        (
            EntityId shoppingCartId,
            EntityId customerId,
            Discount customerDiscount,
            EntityId orderId,
            Guid causationId,
            Guid correlationId
        )
        {
            // Arrange
            IClockService clockService = new ClockService();
            
            ShoppingCart sut = ShoppingCart.Create(shoppingCartId, customerId, customerDiscount, 
                causationId, correlationId).Data;

            // Act
            Result result = sut.Checkout(orderId, clockService.Now);
            
            // Assert
            result.IsError.Should().BeTrue();
        }
        
        [Theory]
        [CustomizedAutoData]
        public void Shopping_cart_checkout_fails_when_not_enough_amount
        (
            EntityId shoppingCartId,
            EntityId customerId,
            Discount customerDiscount,
            EntityId productId,
            EntityId orderId,
            Guid causationId,
            Guid correlationId
        )
        {
            // Arrange
            IClockService clockService = new ClockService();
            
            ShoppingCart sut = ShoppingCart.Create(shoppingCartId, customerId, customerDiscount, 
                causationId, correlationId).Data;
            
            ProductQuantity productQuantity = ProductQuantity.Create(1).Data;
            Price productPrice = Price.Create(ShoppingCart.Settings.MinShoppingCartAmountForCheckout - 1).Data;
            
            sut.AddProductQuantity(productId, productQuantity, productPrice);
            
            // Act
            Result result = sut.Checkout(orderId, clockService.Now);
            
            // Assert
            result.IsError.Should().BeTrue();
        }
        
        [Theory]
        [CustomizedAutoData]
        public void Shopping_cart_checkout_fails_when_not_in_awaiting_confirmation_status
        (
            EntityId shoppingCartId,
            EntityId customerId,
            Discount customerDiscount,
            EntityId productId,
            ProductQuantity productQuantity,
            Price productPrice,
            EntityId orderId,
            Guid causationId,
            Guid correlationId
        )
        {
            // Arrange
            IClockService clockService = new ClockService();
            
            ShoppingCart sut = ShoppingCart.Create(shoppingCartId, customerId, customerDiscount, 
                causationId, correlationId).Data;
            sut.AddProductQuantity(productId, productQuantity, productPrice);
            
            // Act
            Result result = sut.Checkout(orderId, clockService.Now);
            
            // Assert
            result.IsError.Should().BeTrue();
        }
    }
}