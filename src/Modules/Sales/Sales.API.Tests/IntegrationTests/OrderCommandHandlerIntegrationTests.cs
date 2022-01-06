using Xunit;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using EventStore.Client;
using FluentAssertions;

using VShop.SharedKernel.Domain.Enums;
using VShop.SharedKernel.Domain.ValueObjects;
using VShop.SharedKernel.Infrastructure;
using VShop.SharedKernel.Infrastructure.Helpers;
using VShop.SharedKernel.Infrastructure.Services;
using VShop.SharedKernel.Infrastructure.Services.Contracts;
using VShop.SharedKernel.Messaging.Events;
using VShop.SharedKernel.EventSourcing.Stores;
using VShop.SharedKernel.EventStoreDb.Extensions;
using VShop.Modules.Sales.Domain.Enums;
using VShop.Modules.Sales.Domain.Models.Ordering;
using VShop.Modules.Sales.Domain.Models.ShoppingCart;
using VShop.Modules.Sales.Integration.Events;
using VShop.Modules.Sales.Tests.Customizations;
using VShop.Modules.Sales.API.Application.Commands;
using VShop.Modules.Sales.API.Tests.IntegrationTests.Helpers;
using VShop.Modules.Sales.API.Tests.IntegrationTests.Infrastructure;

namespace VShop.Modules.Sales.API.Tests.IntegrationTests
{
    [Collection("Non-Parallel Tests Collection")]
    public class OrderCommandHandlerIntegrationTests
    {
        // Note: PlaceOrderCommand is handler along with the CheckoutShoppingCartCommand.
        
        [Theory]
        [CustomizedAutoData]
        public async Task Cancels_the_order
        (
            ShoppingCart shoppingCart,
            EntityId orderId,
            Guid messageId,
            Guid correlationId
        )
        {
            // Arrange
            IClockService clockService = new ClockService();
            await OrderHelper.PlaceOrderAsync(shoppingCart, orderId, clockService.Now);
            
            CancelOrderCommand command = new(orderId)
            {
                MessageId = messageId,
                CorrelationId = correlationId
            };

            // Act
            Result result = await IntegrationTestsFixture.SendAsync(command);
            
            // Assert
            result.IsError.Should().BeFalse();
            
            Order orderFromDb = await OrderHelper.GetOrderAsync(orderId);
            orderFromDb.Should().NotBeNull();
            orderFromDb.Status.Should().Be(OrderStatus.Cancelled);
        }
        
        [Theory]
        [CustomizedAutoData]
        public async Task Cancelling_the_order_command_is_idempotent
        (
            ShoppingCart shoppingCart,
            EntityId orderId,
            Guid messageId,
            Guid correlationId
        )
        {
            // Arrange
            IClockService clockService = new ClockService();
            await OrderHelper.PlaceOrderAsync(shoppingCart, orderId, clockService.Now);
            
            CancelOrderCommand command = new(orderId)
            {
                MessageId = messageId,
                CorrelationId = correlationId
            };
            await IntegrationTestsFixture.SendAsync(command);
            
            // Act
            Result result = await IntegrationTestsFixture.SendAsync(command);
            
            // Assert
            result.IsError.Should().BeFalse();
        }
        
        [Theory]
        [CustomizedAutoData]
        public async Task Sets_the_order_status_to_paid
        (
            ShoppingCart shoppingCart,
            EntityId orderId,
            Guid messageId,
            Guid correlationId
        )
        {
            // Arrange
            IClockService clockService = new ClockService();
            await OrderHelper.PlaceOrderAsync(shoppingCart, orderId, clockService.Now);
            
            SetPaidOrderStatusCommand command = new(orderId)
            {
                MessageId = messageId,
                CorrelationId = correlationId
            };

            // Act
            Result result = await IntegrationTestsFixture.SendAsync(command);
            
            // Assert
            result.IsError.Should().BeFalse();
            
            Order orderFromDb = await OrderHelper.GetOrderAsync(orderId);
            orderFromDb.Should().NotBeNull();
            orderFromDb.Status.Should().Be(OrderStatus.Paid);
            
            IReadOnlyList<IBaseEvent> orderEvents = await IntegrationTestsFixture
                .ExecuteServiceAsync<EventStoreClient, IReadOnlyList<IBaseEvent>>
                (
                    eventStoreClient => eventStoreClient
                        .ReadStreamForwardAsync<IBaseEvent>(AggregateStore<Order>.GetStreamName(orderId))
                );

            OrderStatusSetToPaidIntegrationEvent orderStatusSetToPaidIntegrationEvent = orderEvents
                .OfType<OrderStatusSetToPaidIntegrationEvent>().SingleOrDefault();
            orderStatusSetToPaidIntegrationEvent.Should().NotBeNull();
        }
        
        [Theory]
        [CustomizedAutoData]
        public async Task Setting_the_order_status_to_paid_command_is_idempotent
        (
            ShoppingCart shoppingCart,
            EntityId orderId,
            Guid messageId,
            Guid correlationId
        )
        {
            // Arrange
            IClockService clockService = new ClockService();
            await OrderHelper.PlaceOrderAsync(shoppingCart, orderId, clockService.Now);
            
            SetPaidOrderStatusCommand command = new(orderId)
            {
                MessageId = messageId,
                CorrelationId = correlationId
            };
            await IntegrationTestsFixture.SendAsync(command);

            // Act
            Result result = await IntegrationTestsFixture.SendAsync(command);
            
            // Assert
            result.IsError.Should().BeFalse();
        }
        
        [Theory]
        [CustomizedAutoData]
        public async Task Finalizes_the_order_when_all_items_are_in_stock
        (
            EntityId shoppingCartId,
            EntityId customerId,
            EntityId firstProductId,
            EntityId secondProductId,
            Address deliveryAddress,
            FullName fullName,
            EmailAddress emailAddress,
            PhoneNumber phoneNumber,
            GenderType genderType,
            EntityId orderId
        )
        {
            // Arrange
            IClockService clockService = new ClockService();
            
            ShoppingCart shoppingCart = ShoppingCart.Create
            (
                shoppingCartId, 
                customerId, 
                Discount.Create(10).Data,
                SequentialGuid.Create(), 
                SequentialGuid.Create()
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
            
            await IntegrationTestsFixture.SendAsync(new SetPaidOrderStatusCommand(orderId)
            {
                MessageId = SequentialGuid.Create(),
                CorrelationId = SequentialGuid.Create()
            });

            FinalizeOrderCommand command = new
            (
                order.Id,
                order.OrderLines.Select(ol => new FinalizeOrderCommand.OrderLine
                {
                    ProductId = ol.Id,
                    OutOfStockQuantity = 0  // All items are in stock.
                }).ToList()
            )
            {
                MessageId = SequentialGuid.Create(),
                CorrelationId = SequentialGuid.Create()
            };

            // Act
            Result result = await IntegrationTestsFixture.SendAsync(command);
            
            // Assert
            result.IsError.Should().BeFalse();
            
             Order orderFromDb = await OrderHelper.GetOrderAsync(orderId);
             orderFromDb.Should().NotBeNull();
             orderFromDb.Status.Should().Be(OrderStatus.PendingShipping);

             IReadOnlyList<IBaseEvent> orderEvents = await IntegrationTestsFixture
                 .ExecuteServiceAsync<EventStoreClient, IReadOnlyList<IBaseEvent>>
                 (
                     eventStoreClient => eventStoreClient
                         .ReadStreamForwardAsync<IBaseEvent>(AggregateStore<Order>.GetStreamName(order.Id))
                 );

             OrderFinalizedIntegrationEvent orderFinalizedIntegrationEvent = orderEvents
                 .OfType<OrderFinalizedIntegrationEvent>().SingleOrDefault();
             orderFinalizedIntegrationEvent.Should().NotBeNull();
             orderFinalizedIntegrationEvent!.RefundAmount.Should().Be(0);
             orderFinalizedIntegrationEvent.OrderLines.Count.Should().Be(2);
        }
        
        [Theory]
        [CustomizedAutoData]
        public async Task Finalizes_the_order_when_all_items_are_out_of_stock
        (
            EntityId shoppingCartId,
            EntityId customerId,
            EntityId firstProductId,
            EntityId secondProductId,
            Address deliveryAddress,
            FullName fullName,
            EmailAddress emailAddress,
            PhoneNumber phoneNumber,
            GenderType genderType,
            EntityId orderId
        )
        {
            // Arrange
            IClockService clockService = new ClockService();
            
            ShoppingCart shoppingCart = ShoppingCart.Create
            (
                shoppingCartId, 
                customerId, 
                Discount.Create(10).Data,
                SequentialGuid.Create(), 
                SequentialGuid.Create()
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
            
            await IntegrationTestsFixture.SendAsync(new SetPaidOrderStatusCommand(orderId)
            {
                MessageId = SequentialGuid.Create(),
                CorrelationId = SequentialGuid.Create()
            });

            FinalizeOrderCommand command = new
            (
                order.Id,
                order.OrderLines.Select(ol => new FinalizeOrderCommand.OrderLine
                {
                    ProductId = ol.Id,
                    OutOfStockQuantity = ol.Quantity
                }).ToList()
            )
            {
                MessageId = SequentialGuid.Create(),
                CorrelationId = SequentialGuid.Create()
            };

            // Act
            Result result = await IntegrationTestsFixture.SendAsync(command);
            
            // Assert
            result.IsError.Should().BeFalse();
            
             Order orderFromDb = await OrderHelper.GetOrderAsync(orderId);
             orderFromDb.Should().NotBeNull();
             orderFromDb.Status.Should().Be(OrderStatus.Cancelled);

             IReadOnlyList<IBaseEvent> orderEvents = await IntegrationTestsFixture
                 .ExecuteServiceAsync<EventStoreClient, IReadOnlyList<IBaseEvent>>
                 (
                     eventStoreClient => eventStoreClient
                         .ReadStreamForwardAsync<IBaseEvent>(AggregateStore<Order>.GetStreamName(order.Id))
                 );

             OrderFinalizedIntegrationEvent orderFinalizedIntegrationEvent = orderEvents
                 .OfType<OrderFinalizedIntegrationEvent>().SingleOrDefault();
             orderFinalizedIntegrationEvent.Should().NotBeNull();
             orderFinalizedIntegrationEvent!.RefundAmount.Should().Be(shoppingCart.FinalAmount);
             orderFinalizedIntegrationEvent.OrderLines.Count.Should().Be(0);
        }
        
        [Theory]
        [CustomizedAutoData]
        public async Task Finalizes_the_order_when_items_are_partially_out_of_stock
        (
            EntityId shoppingCartId,
            EntityId customerId,
            EntityId firstProductId,
            EntityId secondProductId,
            Address deliveryAddress,
            FullName fullName,
            EmailAddress emailAddress,
            PhoneNumber phoneNumber,
            GenderType genderType,
            EntityId orderId
        )
        {
            // Arrange
            IClockService clockService = new ClockService();
            
            ShoppingCart shoppingCart = ShoppingCart.Create
            (
                shoppingCartId, 
                customerId, 
                Discount.Create(10).Data,
                SequentialGuid.Create(), 
                SequentialGuid.Create()
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

            await IntegrationTestsFixture.SendAsync(new SetPaidOrderStatusCommand(orderId)
            {
                MessageId = SequentialGuid.Create(),
                CorrelationId = SequentialGuid.Create()
            });

            IList<FinalizeOrderCommand.OrderLine> finalizedOrderLines = new List<FinalizeOrderCommand.OrderLine>
            {
                new() // -$45
                {
                    ProductId = firstProductId,
                    OutOfStockQuantity = 5
                },
                new() // -$90
                {
                    ProductId = secondProductId,
                    OutOfStockQuantity = 5
                }
            };
            
            // TODO - meesageId and correlationId can be easily forgotten and then there will be a problem.
            FinalizeOrderCommand command = new(orderId, finalizedOrderLines)
            {
                MessageId = SequentialGuid.Create(),
                CorrelationId = SequentialGuid.Create()
            };

            // Act
            Result result = await IntegrationTestsFixture.SendAsync(command);
            
            // Assert
            result.IsError.Should().BeFalse();
            
             Order orderFromDb = await OrderHelper.GetOrderAsync(orderId);
             orderFromDb.Should().NotBeNull();
             orderFromDb.Status.Should().Be(OrderStatus.PendingShipping);

             IReadOnlyList<IBaseEvent> orderEvents = await IntegrationTestsFixture
                 .ExecuteServiceAsync<EventStoreClient, IReadOnlyList<IBaseEvent>>
                 (
                     eventStoreClient => eventStoreClient
                         .ReadStreamForwardAsync<IBaseEvent>(AggregateStore<Order>.GetStreamName(orderId))
                 );

             OrderFinalizedIntegrationEvent orderFinalizedIntegrationEvent = orderEvents
                 .OfType<OrderFinalizedIntegrationEvent>().SingleOrDefault();
             orderFinalizedIntegrationEvent.Should().NotBeNull();
             orderFinalizedIntegrationEvent!.RefundAmount.Should().Be(135);
             orderFinalizedIntegrationEvent.OrderLines.Count.Should().Be(1);
        }
        
        [Theory]
        [CustomizedAutoData]
        public async Task Finalizing_the_order_command_is_idempotent
        (
            ShoppingCart shoppingCart,
            EntityId orderId,
            Guid messageId,
            Guid correlationId
        )
        {
            // Arrange
            IClockService clockService = new ClockService();
            
            Order order = await OrderHelper.PlaceOrderAsync(shoppingCart, orderId, clockService.Now);

            await IntegrationTestsFixture.SendAsync(new SetPaidOrderStatusCommand(orderId)
            {
                MessageId = SequentialGuid.Create(),
                CorrelationId = SequentialGuid.Create()
            });
            
            FinalizeOrderCommand command = new
            (
                order.Id,
                order.OrderLines.Select(ol => new FinalizeOrderCommand.OrderLine
                {
                    ProductId = ol.Id,
                    OutOfStockQuantity = 0
                }).ToList()
            )
            {
                MessageId = messageId,
                CorrelationId = correlationId
            };
            
            await IntegrationTestsFixture.SendAsync(command);

            // Act
            Result result = await IntegrationTestsFixture.SendAsync(command);
            
            // Assert
            result.IsError.Should().BeFalse();
        }
    }
}