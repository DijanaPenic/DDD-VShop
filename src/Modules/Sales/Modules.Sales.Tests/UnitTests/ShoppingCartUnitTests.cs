using Xunit;
using FluentAssertions;

using VShop.Modules.Sales.Domain.Enums;
using VShop.Modules.Sales.Domain.Models.ShoppingCart;
using VShop.SharedKernel.Infrastructure;
using VShop.SharedKernel.Infrastructure.Types;
using VShop.SharedKernel.Infrastructure.Services;
using VShop.SharedKernel.Infrastructure.Services.Contracts;
using VShop.SharedKernel.Domain.ValueObjects;
using VShop.SharedKernel.Tests.Customizations;

namespace VShop.Modules.Sales.Tests.UnitTests
{
    public class ShoppingCartUnitTests
    {
        [Theory, CustomAutoData]
        internal void Creates_a_new_shopping_cart
        (
            EntityId shoppingCartId,
            EntityId customerId,
            Discount customerDiscount
        )
        {
            // Act
            Result<ShoppingCart> result = ShoppingCart.Create
            (
                shoppingCartId,
                customerId,
                customerDiscount
            );
            
            // Assert
            result.IsError.Should().BeFalse();

            ShoppingCart shoppingCart = result.Data;
            shoppingCart.Should().NotBeNull();
            shoppingCart.Status.Should().Be(ShoppingCartStatus.New);
            shoppingCart.DeliveryCost.Value.Should().Be(ShoppingCart.Settings.DefaultDeliveryCost);
        }
        
        [Theory, CustomAutoData]
        internal void Sets_product_price
        (
            ShoppingCart sut,
            EntityId productId,
            ProductQuantity productQuantity,
            Price productPrice
        )
        {
            // Arrange
            sut.AddProductQuantity(productId, productQuantity, productPrice);
            Price newProductPrice = ++productPrice;
            
            // Act
            Result result = sut.SetProductPrice(productId, newProductPrice);
            
            // Assert
            result.IsError.Should().BeFalse();
            
            ShoppingCartItem shoppingCartItem = sut.Items
                .FirstOrDefault(p => Equals(p.Id, productId));
            shoppingCartItem.Should().NotBeNull();
            shoppingCartItem!.UnitPrice.Should().Be(newProductPrice);
        }
        
        [Theory, CustomAutoData]
        internal void Setting_product_price_fails_when_shopping_cart_is_closed_for_updates
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
            
            sut.AddProductQuantity(productId, productQuantity, productPrice);
            Price newProductPrice = ++productPrice;

            sut.Checkout(orderId, clockService.Now); // Checkout will prevent further updates

            // Act
            Result result = sut.SetProductPrice(productId, newProductPrice);
            
            // Assert
            result.IsError.Should().BeTrue();
        }
        
        [Theory, CustomAutoData]
        internal void Setting_product_price_fails_when_product_is_missing
        (
            ShoppingCart sut,
            EntityId productId,
            Price productPrice
        )
        {
            // Act
            Result result = sut.SetProductPrice(productId, productPrice);
            
            // Assert
            result.IsError.Should().BeTrue();
        }
        
        [Theory, CustomAutoData]
        internal void Delivery_cost_is_applied_when_changing_product_price_and_not_enough_purchase_amount
        (
            EntityId shoppingCartId,
            EntityId customerId,
            EntityId productId
        )
        {
            // Arrange
            ShoppingCart sut = ShoppingCart
                .Create(shoppingCartId, customerId, Discount.Create(0).Data).Data;

            ProductQuantity productQuantity = ProductQuantity.Create(1).Data;
            Price initialProductPrice = Price.Create(ShoppingCart.Settings.MinShoppingCartAmountForFreeDelivery).Data;
            
            sut.AddProductQuantity(productId, productQuantity, initialProductPrice);
            
            Price newProductPrice = Price.Create(ShoppingCart.Settings.MinShoppingCartAmountForFreeDelivery - 1).Data;

            // Act
            Result result = sut.SetProductPrice(productId, newProductPrice);
            
            // Assert
            result.IsError.Should().BeFalse();
            sut.DeliveryCost.Value.Should().Be(ShoppingCart.Settings.DefaultDeliveryCost);
        }
        
        [Theory, CustomAutoData]
        internal void Delivery_cost_is_zero_when_changing_product_price_and_enough_purchase_amount
        (
            EntityId shoppingCartId,
            EntityId customerId,
            EntityId productId
        )
        {
            // Arrange
            ShoppingCart sut = ShoppingCart
                .Create(shoppingCartId, customerId, Discount.Create(0).Data).Data;

            ProductQuantity productQuantity = ProductQuantity.Create(1).Data;
            Price initialProductPrice = Price.Create(ShoppingCart.Settings.MinShoppingCartAmountForFreeDelivery - 1).Data;
            
            sut.AddProductQuantity(productId, productQuantity, initialProductPrice);
            
            Price newProductPrice = Price.Create(ShoppingCart.Settings.MinShoppingCartAmountForFreeDelivery).Data;

            // Act
            Result result = sut.SetProductPrice(productId, newProductPrice);
            
            // Assert
            result.IsError.Should().BeFalse();
            sut.DeliveryCost.Value.Should().Be(0);
        }
        
        [Theory, CustomAutoData]
        internal void Product_quantity_increment_adds_a_new_product_to_the_shopping_cart
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
        
        [Theory, CustomAutoData]
        internal void Product_quantity_increment_increases_the_existing_product_quantity           
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

        
        [Theory, CustomAutoData]
        internal void  Product_quantity_increment_fails_when_shopping_cart_is_closed_for_updates
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

        [Theory, CustomAutoData]
        internal void Product_quantity_increment_fails_when_price_mismatch
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
        
        [Theory, CustomAutoData]
        internal void Product_quantity_increment_fails_when_per_product_quantity_is_more_than_allowed
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
        
        [Theory, CustomAutoData]
        internal void Delivery_cost_is_applied_when_adding_product_quantity_and_not_enough_purchase_amount
        (
            EntityId shoppingCartId,
            EntityId customerId,
            EntityId productId
        )
        {
            // Arrange
            ShoppingCart sut = ShoppingCart
                .Create(shoppingCartId, customerId, Discount.Create(0).Data).Data;

            ProductQuantity productQuantity = ProductQuantity.Create(1).Data;
            Price productPrice = Price.Create(ShoppingCart.Settings.MinShoppingCartAmountForFreeDelivery - 1).Data;

            // Act
            Result result = sut.AddProductQuantity(productId, productQuantity, productPrice);
            
            // Assert
            result.IsError.Should().BeFalse();
            sut.DeliveryCost.Value.Should().Be(ShoppingCart.Settings.DefaultDeliveryCost);
        }
        
        [Theory, CustomAutoData]
        internal void Delivery_cost_is_zero_when_adding_product_quantity_and_enough_purchase_amount
        (
            EntityId shoppingCartId,
            EntityId customerId,
            EntityId productId
        )
        {
            // Arrange
            ShoppingCart sut = ShoppingCart
                .Create(shoppingCartId, customerId, Discount.Create(0).Data).Data;

            ProductQuantity productQuantity = ProductQuantity.Create(1).Data;
            Price productPrice = Price.Create(ShoppingCart.Settings.MinShoppingCartAmountForFreeDelivery).Data;

            // Act
            Result result = sut.AddProductQuantity(productId, productQuantity, productPrice);
            
            // Assert
            result.IsError.Should().BeFalse();
            sut.DeliveryCost.Value.Should().Be(0m);
        }
        
        [Theory, CustomAutoData]
        internal void Product_quantity_removal_removes_product_from_the_shopping_cart(ShoppingCart sut)
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
        
        [Theory, CustomAutoData]
        internal void Product_quantity_removal_decreases_the_existing_product_quantity           
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
        
        [Theory, CustomAutoData]
        internal void Product_quantity_removal_fails_when_shopping_cart_is_closed_for_updates(ShoppingCart sut, EntityId orderId)
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
        
        [Theory, CustomAutoData]
        internal void Delivery_cost_is_applied_when_removing_product_quantity_and_not_enough_purchase_amount
        (
            EntityId shoppingCartId,
            EntityId customerId,
            EntityId productId
        )
        {
            // Arrange
            ShoppingCart sut = ShoppingCart
                .Create(shoppingCartId, customerId, Discount.Create(0).Data).Data;

            ProductQuantity productQuantity = ProductQuantity.Create(2).Data;
            Price productPrice = Price.Create(ShoppingCart.Settings.MinShoppingCartAmountForFreeDelivery - 1).Data;
            
            sut.AddProductQuantity(productId, productQuantity, productPrice);

            // Act
            Result result = sut.RemoveProductQuantity(productId, ProductQuantity.Create(1).Data);
            
            // Assert
            result.IsError.Should().BeFalse();
            sut.DeliveryCost.Value.Should().Be(ShoppingCart.Settings.DefaultDeliveryCost);
        }
        
        [Theory, CustomAutoData]
        internal void Decreasing_the_existing_product_quantity_fails_when_too_high_removal_quantity
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
        
        [Theory, CustomAutoData]
        internal void Deletes_the_shopping_cart(ShoppingCart sut)
        {
            // Act
            Result result = sut.Delete();
            
            // Assert
            result.IsError.Should().BeFalse();
            sut.Status.Should().Be(ShoppingCartStatus.Closed);
        }
        
        [Theory, CustomAutoData]
        internal void Shopping_cart_delete_fails_for_already_deleted_shopping_cart(ShoppingCart sut)
        {
            // Arrange
            sut.Delete();

            // Act
            Result result = sut.Delete();
            
            // Assert
            result.IsError.Should().BeTrue();
        }
        
        [Theory, CustomAutoData]
        internal void Sets_customer_contact_information
        (
            ShoppingCart shoppingCart,
            FullName fullName,
            EmailAddress emailAddress,
            PhoneNumber phoneNumber,
            Gender genderType
        )
        {
            // Arrange
            ShoppingCartCustomer sut = shoppingCart.Customer;
            
            // Act
            Result result = sut.SetContactInformation(fullName, emailAddress, phoneNumber, genderType);
            
            // Assert
            result.IsError.Should().BeFalse();
        }
        
        [Theory, CustomAutoData]
        internal void Setting_customer_contact_information_fails_when_shopping_cart_is_closed_for_updates
        (
            ShoppingCart shoppingCart,
            EntityId orderId,
            FullName fullName,
            EmailAddress emailAddress,
            PhoneNumber phoneNumber,
            Gender genderType
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
        
        [Theory, CustomAutoData]
        internal void Sets_customer_delivery_address
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
        
        [Theory, CustomAutoData]
        internal void Setting_customer_delivery_address_fails_when_shopping_cart_is_closed_for_updates
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
        
        [Theory, CustomAutoData]
        internal void Checkouts_the_shopping_cart(ShoppingCart sut, EntityId orderId)
        {
            // Arrange
            IClockService clockService = new ClockService();

            // Act
            Result result = sut.Checkout(orderId, clockService.Now);
            
            // Assert
            result.IsError.Should().BeFalse();
            sut.Status.Should().Be(ShoppingCartStatus.PendingCheckout);
        }

        [Theory, CustomAutoData]
        internal void Shopping_cart_checkout_fails_when_shopping_cart_is_empty
        (
            EntityId shoppingCartId,
            EntityId customerId,
            Discount customerDiscount,
            EntityId orderId
        )
        {
            // Arrange
            IClockService clockService = new ClockService();
            
            ShoppingCart sut = ShoppingCart.Create(shoppingCartId, customerId, customerDiscount).Data;

            // Act
            Result result = sut.Checkout(orderId, clockService.Now);
            
            // Assert
            result.IsError.Should().BeTrue();
        }
        
        [Theory, CustomAutoData]
        internal void Shopping_cart_checkout_fails_when_not_enough_amount
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
            
            ShoppingCart sut = ShoppingCart.Create(shoppingCartId, customerId, customerDiscount).Data;
            
            ProductQuantity productQuantity = ProductQuantity.Create(1).Data;
            Price productPrice = Price.Create(ShoppingCart.Settings.MinShoppingCartAmountForCheckout - 1).Data;
            
            sut.AddProductQuantity(productId, productQuantity, productPrice);
            
            // Act
            Result result = sut.Checkout(orderId, clockService.Now);
            
            // Assert
            result.IsError.Should().BeTrue();
        }
        
        [Theory, CustomAutoData]
        internal void Shopping_cart_checkout_fails_when_not_in_awaiting_confirmation_status
        (
            EntityId shoppingCartId,
            EntityId customerId,
            Discount customerDiscount,
            EntityId productId,
            ProductQuantity productQuantity,
            Price productPrice,
            EntityId orderId
        )
        {
            // Arrange
            IClockService clockService = new ClockService();
            
            ShoppingCart sut = ShoppingCart.Create(shoppingCartId, customerId, customerDiscount).Data;
            sut.AddProductQuantity(productId, productQuantity, productPrice);
            
            // Act
            Result result = sut.Checkout(orderId, clockService.Now);
            
            // Assert
            result.IsError.Should().BeTrue();
        }
    }
}