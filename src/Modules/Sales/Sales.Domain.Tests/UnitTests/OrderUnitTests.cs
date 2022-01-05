using Xunit;
using System;
using System.Linq;
using FluentAssertions;

using VShop.SharedKernel.Infrastructure;
using VShop.SharedKernel.Domain.ValueObjects;
using VShop.Modules.Sales.Domain.Enums;
using VShop.Modules.Sales.Domain.Models.Ordering;
using VShop.Modules.Sales.Tests.Customizations;

namespace VShop.Modules.Sales.Domain.Tests.UnitTests
{
    public class OrderUnitTests
    {
        [Theory]
        [CustomizedAutoData]
        public void Creates_an_order
        (
            EntityId orderId,
            Price deliveryCost,
            EntityId customerId,
            Discount customerDiscount,
            FullName fullName,
            EmailAddress emailAddress,
            PhoneNumber phoneNumber,
            Address deliveryAddress,
            Guid causationId,
            Guid correlationId
        )
        {
            // Act
            Result<Order> result = Order.Create
            (
                orderId,
                deliveryCost,
                customerId,
                customerDiscount,
                fullName,
                emailAddress,
                phoneNumber,
                deliveryAddress,
                causationId,
                correlationId
            );
            
            // Assert
            result.IsError.Should().BeFalse();

            Order order = result.Data;
            order.Should().NotBeNull();
            order.Status.Should().Be(OrderStatus.Processing);
        }
        
        [Theory]
        [CustomizedAutoData]
        public void Adds_a_new_order_line_to_order
        (
            Order sut,
            EntityId productId,
            ProductQuantity productQuantity,
            Price productPrice
        )
        {
            // Act
            Result result = sut.AddOrderLine(productId, productQuantity, productPrice);
            
            // Assert
            result.IsError.Should().BeFalse();
            
            OrderLine orderLine = sut.OrderLines
                .FirstOrDefault(p => Equals(p.Id, productId));
            orderLine.Should().NotBeNull();
        }
        
        [Theory]
        [CustomizedAutoData]
        public void Removes_out_of_stock_items_from_order
        (
            Order sut,
            EntityId productId,
            Price productPrice
        )
        {
            // Arrange
            sut.AddOrderLine(productId, ProductQuantity.Create(10).Data, productPrice);
            ProductQuantity removeQuantity = ProductQuantity.Create(5).Data;
            
            // Act
            Result result = sut.RemoveOutOfStockItems(productId, removeQuantity);
            
            // Assert
            result.IsError.Should().BeFalse();

            OrderLine orderLine = sut.OrderLines
                .FirstOrDefault(p => Equals(p.Id, productId));
            orderLine.Should().NotBeNull();
            orderLine!.Quantity.Value.Should().Be(5);
        }
        
        [Theory]
        [CustomizedAutoData]
        public void Removing_out_of_stock_items_from_order_fails_when_too_high_removal_quantity
        (
            Order sut,
            EntityId productId,
            Price productPrice
        )
        {
            // Arrange
            sut.AddOrderLine(productId, ProductQuantity.Create(10).Data, productPrice);
            ProductQuantity removeQuantity = ProductQuantity.Create(15).Data;
            
            // Act
            Result result = sut.RemoveOutOfStockItems(productId, removeQuantity);
            
            // Assert
            result.IsError.Should().BeTrue();
        }
        
        [Theory]
        [CustomizedAutoData]
        public void Changes_status_from_processing_to_paid(Order sut)
        {
            // Act
            Result result = sut.SetPaidStatus();
            
            // Assert
            result.IsError.Should().BeFalse();
            sut.Status.Should().Be(OrderStatus.Paid);
        }
        
        [Theory]
        [CustomizedAutoData]
        public void Changing_status_from_pending_shipping_to_paid_fails(Order sut)
        {
            // Arrange
            sut.SetPaidStatus();
            sut.SetPendingShippingStatus();

            // Act
            Result result = sut.SetPaidStatus();
            
            // Assert
            result.IsError.Should().BeTrue();
        }
        
        [Theory]
        [CustomizedAutoData]
        public void Changing_status_from_paid_to_pending_shipping_succeeds(Order sut)
        {
            // Arrange
            sut.SetPaidStatus();
            
            // Act
            Result result = sut.SetPendingShippingStatus();
            
            // Assert
            result.IsError.Should().BeFalse();
            sut.Status.Should().Be(OrderStatus.PendingShipping);
        }
        
        [Theory]
        [CustomizedAutoData]
        public void Changing_status_from_processing_to_pending_shipping_fails(Order sut)
        {
            // Act
            Result result = sut.SetPendingShippingStatus();
            
            // Assert
            result.IsError.Should().BeTrue();
        }
        
        [Theory]
        [CustomizedAutoData]
        public void Changing_status_from_pending_shipping_to_shipped_succeeds(Order sut)
        {
            // Arrange
            sut.SetPaidStatus();
            sut.SetPendingShippingStatus();
            
            // Act
            Result result = sut.SetShippedStatus();
            
            // Assert
            result.IsError.Should().BeFalse();
            sut.Status.Should().Be(OrderStatus.Shipped);
        }
        
        [Theory]
        [CustomizedAutoData]
        public void Changing_status_from_processing_to_shipped_fails(Order sut)
        {
            // Act
            Result result = sut.SetShippedStatus();
            
            // Assert
            result.IsError.Should().BeTrue();
        }
        
        [Theory]
        [CustomizedAutoData]
        public void Changing_status_from_processing_to_cancelled_succeeds(Order sut)
        {
            // Act
            Result result = sut.SetCancelledStatus();
            
            // Assert
            result.IsError.Should().BeFalse();
            sut.Status.Should().Be(OrderStatus.Cancelled);
        }
        
        [Theory]
        [CustomizedAutoData]
        public void Changing_status_from_paid_to_cancelled_succeeds(Order sut)
        {
            // Arrange
            sut.SetPaidStatus();
            
            // Act
            Result result = sut.SetCancelledStatus();
            
            // Assert
            result.IsError.Should().BeFalse();
            sut.Status.Should().Be(OrderStatus.Cancelled);
        }

        [Theory]
        [CustomizedAutoData]
        public void Changing_status_from_pending_shipping_to_cancelled_fails(Order sut)
        {
            // Arrange 
            sut.SetPaidStatus();
            sut.SetPendingShippingStatus();
            
            // Act
            Result result = sut.SetCancelledStatus();
            
            // Assert
            result.IsError.Should().BeTrue();
        }
    }
}