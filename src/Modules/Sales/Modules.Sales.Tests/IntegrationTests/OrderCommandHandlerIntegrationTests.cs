using Xunit;
using FluentAssertions;

using VShop.Modules.Sales.Domain.Enums;
using VShop.Modules.Sales.Domain.Models.Ordering;
using VShop.Modules.Sales.Domain.Models.ShoppingCart;
using VShop.Modules.Sales.Infrastructure.Commands;
using VShop.Modules.Sales.Integration.Events;
using VShop.Modules.Sales.Tests.IntegrationTests.Helpers;
using VShop.Modules.Sales.Tests.IntegrationTests.Infrastructure;
using VShop.SharedKernel.Domain.ValueObjects;
using VShop.SharedKernel.EventSourcing.Stores;
using VShop.SharedKernel.EventStoreDb;
using VShop.SharedKernel.Infrastructure;
using VShop.SharedKernel.Infrastructure.Contexts.Contracts;
using VShop.SharedKernel.Infrastructure.Events.Contracts;
using VShop.SharedKernel.Infrastructure.Messaging;
using VShop.SharedKernel.Infrastructure.Services;
using VShop.SharedKernel.Infrastructure.Services.Contracts;
using VShop.SharedKernel.Infrastructure.Types;
using VShop.SharedKernel.Tests.Customizations;

namespace VShop.Modules.Sales.Tests.IntegrationTests
{
    [Collection("Non-Parallel Tests Collection")]
    public class OrderCommandHandlerIntegrationTests : TestBase
    {
        [Theory, CustomAutoData]
        internal async Task Creates_a_new_order_from_the_shopping_cart
        (
            ShoppingCart shoppingCart,
            EntityId orderId
        )
        {
            // Arrange
            IClockService clockService = new ClockService();
            
            shoppingCart.Checkout(orderId, clockService.Now);
            await ShoppingCartHelper.SaveAsync(shoppingCart);
            
            PlaceOrderCommand command = new(orderId, shoppingCart.Id);

            // Act
            Result<Order> result = await SalesModule.SendAsync(command);
            
            // Assert
            result.IsError.Should().BeFalse();
            
            Order orderFromDb = await OrderHelper.GetOrderAsync(orderId);
            orderFromDb.Should().NotBeNull();
            orderFromDb.Status.Should().Be(OrderStatus.Processing);
        }
        
        [Theory, CustomAutoData]
        internal async Task Creating_a_new_order_from_the_shopping_cart_is_idempotent
        (
            ShoppingCart shoppingCart,
            EntityId orderId
        )
        {
            // Arrange
            IClockService clockService = new ClockService();
            
            shoppingCart.Checkout(orderId, clockService.Now);
            await ShoppingCartHelper.SaveAsync(shoppingCart);
            
            PlaceOrderCommand command = new(orderId, shoppingCart.Id);
            
            await SalesModule.SendAsync(command);

            // Act
            Result<Order> result = await SalesModule.SendAsync(command);
            
            // Assert
            result.IsError.Should().BeFalse();
        }
        
        [Theory, CustomAutoData]
        internal async Task Cancels_the_order
        (
            ShoppingCart shoppingCart,
            EntityId orderId
        )
        {
            // Arrange
            IClockService clockService = new ClockService();
            await OrderHelper.PlaceOrderAsync(shoppingCart, orderId, clockService.Now);
            
            CancelOrderCommand command = new(orderId);

            // Act
            Result result = await SalesModule.SendAsync(command);
            
            // Assert
            result.IsError.Should().BeFalse();
            
            Order orderFromDb = await OrderHelper.GetOrderAsync(orderId);
            orderFromDb.Should().NotBeNull();
            orderFromDb.Status.Should().Be(OrderStatus.Cancelled);
        }
        
        [Theory, CustomAutoData]
        internal async Task Cancelling_the_order_command_is_idempotent
        (
            ShoppingCart shoppingCart,
            EntityId orderId,
            IContext context
        )
        {
            // Arrange
            IClockService clockService = new ClockService();
            await OrderHelper.PlaceOrderAsync(shoppingCart, orderId, clockService.Now);
            
            CancelOrderCommand command = new(orderId);
            
            await SalesModule.SendAsync(command, context);
            
            // Act
            Result result = await SalesModule.SendAsync(command, context);
            
            // Assert
            result.IsError.Should().BeFalse();
        }
        
        [Theory, CustomAutoData]
        internal async Task Sets_the_order_status_to_paid
        (
            ShoppingCart shoppingCart,
            EntityId orderId
        )
        {
            // Arrange
            IClockService clockService = new ClockService();
            await OrderHelper.PlaceOrderAsync(shoppingCart, orderId, clockService.Now);
            
            SetPaidOrderStatusCommand command = new(orderId);

            // Act
            Result result = await SalesModule.SendAsync(command);
            
            // Assert
            result.IsError.Should().BeFalse();
            
            Order orderFromDb = await OrderHelper.GetOrderAsync(orderId);
            orderFromDb.Should().NotBeNull();
            orderFromDb.Status.Should().Be(OrderStatus.Paid);
            
            IReadOnlyList<MessageEnvelope<IBaseEvent>> orderEvents = await SalesModule
                .ExecuteServiceAsync<CustomEventStoreClient, IReadOnlyList<MessageEnvelope<IBaseEvent>>>
                (
                    eventStoreClient => eventStoreClient
                        .ReadStreamForwardAsync<IBaseEvent>(AggregateStore<Order>.GetStreamName(orderId))
                );

            OrderStatusSetToPaidIntegrationEvent orderStatusSetToPaidIntegrationEvent = orderEvents.ToMessages()
                .OfType<OrderStatusSetToPaidIntegrationEvent>().SingleOrDefault();
            orderStatusSetToPaidIntegrationEvent.Should().NotBeNull();
        }
        
        [Theory, CustomAutoData]
        internal async Task Setting_the_order_status_to_paid_command_is_idempotent
        (
            ShoppingCart shoppingCart,
            EntityId orderId,
            IContext context
        )
        {
            // Arrange
            IClockService clockService = new ClockService();
            await OrderHelper.PlaceOrderAsync(shoppingCart, orderId, clockService.Now);

            SetPaidOrderStatusCommand command = new(orderId);
            await SalesModule.SendAsync(command, context);

            // Act
            Result result = await SalesModule.SendAsync(command, context);
            
            // Assert
            result.IsError.Should().BeFalse();
        }
        
        [Theory, CustomAutoData]
        internal async Task Finalizes_the_order_when_all_items_are_in_stock
        (
            EntityId shoppingCartId,
            EntityId customerId,
            EntityId firstProductId,
            EntityId secondProductId,
            Address deliveryAddress,
            FullName fullName,
            EmailAddress emailAddress,
            PhoneNumber phoneNumber,
            Gender genderType,
            EntityId orderId
        )
        {
            // Arrange
            IClockService clockService = new ClockService();
            
            ShoppingCart shoppingCart = ShoppingCart.Create
            (
                shoppingCartId, 
                customerId, 
                Discount.Create(10).Data
            ).Data;

            shoppingCart.AddProductQuantity
            (
                firstProductId,
                ProductQuantity.Create(10).Data,
                Price.Create(10).Data
            );
            shoppingCart.AddProductQuantity
            (
                secondProductId,
                ProductQuantity.Create(5).Data,
                Price.Create(20).Data
            );
            shoppingCart.Customer.SetDeliveryAddress(deliveryAddress);
            shoppingCart.Customer.SetContactInformation(fullName, emailAddress, phoneNumber, genderType);
            
            Order order = await OrderHelper.PlaceOrderAsync(shoppingCart, orderId, clockService.Now);
            
            await SalesModule.SendAsync(new SetPaidOrderStatusCommand(order.Id));

            FinalizeOrderCommand command = new
            (
                order.Id,
                order.OrderLines.Select(ol => new FinalizeOrderCommand.Types.OrderLine
                {
                    ProductId = ol.Id.Value,
                    OutOfStockQuantity = 0  // All items are in stock.
                }).ToList()
            );

            // Act
            Result result = await SalesModule.SendAsync(command);
            
            // Assert
            result.IsError.Should().BeFalse();
            
             Order orderFromDb = await OrderHelper.GetOrderAsync(orderId);
             orderFromDb.Should().NotBeNull();
             orderFromDb.Status.Should().Be(OrderStatus.PendingShipping);

             IReadOnlyList<MessageEnvelope<IBaseEvent>> orderEvents = await SalesModule
                 .ExecuteServiceAsync<CustomEventStoreClient, IReadOnlyList<MessageEnvelope<IBaseEvent>>>
                 (
                     eventStoreClient => eventStoreClient
                         .ReadStreamForwardAsync<IBaseEvent>(AggregateStore<Order>.GetStreamName(order.Id))
                 );

             OrderFinalizedIntegrationEvent orderFinalizedIntegrationEvent = orderEvents.ToMessages()
                 .OfType<OrderFinalizedIntegrationEvent>().SingleOrDefault();
             orderFinalizedIntegrationEvent.Should().NotBeNull();
             orderFinalizedIntegrationEvent!.RefundAmount.DecimalValue.Should().Be(0);
             orderFinalizedIntegrationEvent.OrderLines.Count.Should().Be(2);
        }
        
        [Theory, CustomAutoData]
        internal async Task Finalizes_the_order_when_all_items_are_out_of_stock
        (
            EntityId shoppingCartId,
            EntityId customerId,
            EntityId firstProductId,
            EntityId secondProductId,
            Address deliveryAddress,
            FullName fullName,
            EmailAddress emailAddress,
            PhoneNumber phoneNumber,
            Gender genderType,
            EntityId orderId
        )
        {
            // Arrange
            IClockService clockService = new ClockService();
            
            ShoppingCart shoppingCart = ShoppingCart.Create
            (
                shoppingCartId, 
                customerId, 
                Discount.Create(10).Data
            ).Data;

            shoppingCart.AddProductQuantity
            (
                firstProductId,
                ProductQuantity.Create(10).Data,
                Price.Create(10).Data
            );
            shoppingCart.AddProductQuantity
            (
                secondProductId,
                ProductQuantity.Create(5).Data,
                Price.Create(20).Data
            );
            shoppingCart.Customer.SetDeliveryAddress(deliveryAddress);
            shoppingCart.Customer.SetContactInformation(fullName, emailAddress, phoneNumber, genderType);
            
            Order order = await OrderHelper.PlaceOrderAsync(shoppingCart, orderId, clockService.Now);
            
            await SalesModule.SendAsync(new SetPaidOrderStatusCommand(order.Id));
            FinalizeOrderCommand command = new
            (
                order.Id,
                order.OrderLines.Select(ol => new FinalizeOrderCommand.Types.OrderLine
                {
                    ProductId = ol.Id.Value,
                    OutOfStockQuantity = ol.Quantity
                }).ToList()
            );

            // Act
            Result result = await SalesModule.SendAsync(command);
            
            // Assert
            result.IsError.Should().BeFalse();
            
             Order orderFromDb = await OrderHelper.GetOrderAsync(orderId);
             orderFromDb.Should().NotBeNull();
             orderFromDb.Status.Should().Be(OrderStatus.Cancelled);

             IReadOnlyList<MessageEnvelope<IBaseEvent>> orderEvents = await SalesModule
                 .ExecuteServiceAsync<CustomEventStoreClient, IReadOnlyList<MessageEnvelope<IBaseEvent>>>
                 (
                     eventStoreClient => eventStoreClient
                         .ReadStreamForwardAsync<IBaseEvent>(AggregateStore<Order>.GetStreamName(order.Id))
                 );

             OrderFinalizedIntegrationEvent orderFinalizedIntegrationEvent = orderEvents.ToMessages()
                 .OfType<OrderFinalizedIntegrationEvent>().SingleOrDefault();
             orderFinalizedIntegrationEvent.Should().NotBeNull();
             orderFinalizedIntegrationEvent!.RefundAmount.DecimalValue.Should().Be(shoppingCart.FinalAmount);
             orderFinalizedIntegrationEvent.OrderLines.Count.Should().Be(0);
        }
        
        [Theory, CustomAutoData]
        internal async Task Finalizes_the_order_when_items_are_partially_out_of_stock
        (
            EntityId shoppingCartId,
            EntityId customerId,
            EntityId firstProductId,
            EntityId secondProductId,
            Address deliveryAddress,
            FullName fullName,
            EmailAddress emailAddress,
            PhoneNumber phoneNumber,
            Gender genderType,
            EntityId orderId
        )
        {
            // Arrange
            IClockService clockService = new ClockService();
            
            ShoppingCart shoppingCart = ShoppingCart.Create
            (
                shoppingCartId, 
                customerId, 
                Discount.Create(10).Data
            ).Data;

            shoppingCart.AddProductQuantity // +$90 
            (
                firstProductId,
                ProductQuantity.Create(10).Data,
                Price.Create(10).Data
            );
            shoppingCart.AddProductQuantity // +$90
            (
                secondProductId,
                ProductQuantity.Create(5).Data,
                Price.Create(20).Data
            );
            shoppingCart.Customer.SetDeliveryAddress(deliveryAddress);
            shoppingCart.Customer.SetContactInformation(fullName, emailAddress, phoneNumber, genderType);
            
            await OrderHelper.PlaceOrderAsync(shoppingCart, orderId, clockService.Now);

            await SalesModule.SendAsync(new SetPaidOrderStatusCommand(orderId));

            IList<FinalizeOrderCommand.Types.OrderLine> finalizedOrderLines = new List<FinalizeOrderCommand.Types.OrderLine>
            {
                new() // -$45
                {
                    ProductId = firstProductId.Value,
                    OutOfStockQuantity = 5
                },
                new() // -$90
                {
                    ProductId = secondProductId.Value,
                    OutOfStockQuantity = 5
                }
            };
            
            FinalizeOrderCommand command = new(orderId, finalizedOrderLines);

            // Act
            Result result = await SalesModule.SendAsync(command);
            
            // Assert
            result.IsError.Should().BeFalse();
            
             Order orderFromDb = await OrderHelper.GetOrderAsync(orderId);
             orderFromDb.Should().NotBeNull();
             orderFromDb.Status.Should().Be(OrderStatus.PendingShipping);

             IReadOnlyList<MessageEnvelope<IBaseEvent>> orderEvents = await SalesModule
                 .ExecuteServiceAsync<CustomEventStoreClient, IReadOnlyList<MessageEnvelope<IBaseEvent>>>
                 (
                     eventStoreClient => eventStoreClient
                         .ReadStreamForwardAsync<IBaseEvent>(AggregateStore<Order>.GetStreamName(orderId))
                 );

             OrderFinalizedIntegrationEvent orderFinalizedIntegrationEvent = orderEvents.ToMessages()
                 .OfType<OrderFinalizedIntegrationEvent>().SingleOrDefault();
             orderFinalizedIntegrationEvent.Should().NotBeNull();
             orderFinalizedIntegrationEvent!.RefundAmount.DecimalValue.Should().Be(135);
             orderFinalizedIntegrationEvent.OrderLines.Count.Should().Be(1);
        }
        
        [Theory, CustomAutoData]
        internal async Task Finalizing_the_order_command_is_idempotent
        (
            ShoppingCart shoppingCart,
            EntityId orderId,
            IContext context
        )
        {
            // Arrange
            IClockService clockService = new ClockService();
            
            Order order = await OrderHelper.PlaceOrderAsync(shoppingCart, orderId, clockService.Now);

            await SalesModule.SendAsync(new SetPaidOrderStatusCommand(orderId));
            
            FinalizeOrderCommand command = new
            (
                order.Id,
                order.OrderLines.Select(ol => new FinalizeOrderCommand.Types.OrderLine
                {
                    ProductId = ol.Id.Value,
                    OutOfStockQuantity = 0
                }).ToList()
            );
            
            await SalesModule.SendAsync(command, context);

            // Act
            Result result = await SalesModule.SendAsync(command, context);
            
            // Assert
            result.IsError.Should().BeFalse();
        }
    }
}