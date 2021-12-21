using Xunit;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

using VShop.SharedKernel.Messaging;
using VShop.SharedKernel.Infrastructure;
using VShop.SharedKernel.Infrastructure.Services;
using VShop.SharedKernel.Infrastructure.Services.Contracts;
using VShop.SharedKernel.Domain.ValueObjects;
using VShop.SharedKernel.Scheduler.Infrastructure;
using VShop.SharedKernel.Scheduler.Infrastructure.Entities;
using VShop.Modules.Sales.Domain.Enums;
using VShop.Modules.Sales.Domain.Events;
using VShop.Modules.Sales.Domain.Models.Ordering;
using VShop.Modules.Sales.Domain.Models.ShoppingCart;
using VShop.Modules.Sales.Tests.Customizations;
using VShop.Modules.Sales.API.Application.Commands;
using VShop.Modules.Sales.API.Application.ProcessManagers;
using VShop.Modules.Sales.API.Tests.IntegrationTests.Helpers;
using VShop.Modules.Sales.API.Tests.IntegrationTests.Infrastructure;
using VShop.Modules.Billing.Integration.Events;

namespace VShop.Modules.Sales.API.Tests.IntegrationTests
{
    [Collection("Non-Parallel Tests Collection")]
    public class OrderingIntegrationTests
    {
        [Theory]
        [CustomizedAutoData]
        public async Task Shopping_cart_checkout_places_an_order(ShoppingCart shoppingCart)
        {
            // Arrange
            await ShoppingCartHelper.SaveAndPublishAsync(shoppingCart);

            CheckoutShoppingCartCommand command = new(shoppingCart.Id);
            
            // Act
            Result<CheckoutOrder> result = await IntegrationTestsFixture.SendAsync(command);
            
            // Assert
            result.IsError.Should().BeFalse();
            
            ShoppingCart shoppingCartFromDb = await ShoppingCartHelper.GetShoppingCartAsync(EntityId.Create(command.ShoppingCartId).Value);
            shoppingCartFromDb.Status.Should().Be(ShoppingCartStatus.Closed); // The shopping cart should have been deleted.
            
            EntityId orderId = EntityId.Create(result.Value.OrderId).Value;

            Order orderFromDb = await OrderHelper.GetOrderAsync(orderId);
            orderFromDb.Should().NotBeNull(); // The order should have been created.
        }
        
        [Theory]
        [CustomizedAutoData]
        public async Task Payment_failure_schedules_a_reminder_message(EntityId orderId, ShoppingCart shoppingCart)
        {
            // Arrange
            IClockService clockService = new ClockService();

            await OrderHelper.PlaceOrderAsync(shoppingCart, orderId, clockService.Now);
        
            PaymentFailedIntegrationEvent failedPaymentIntegrationEvent = new(orderId);
        
            // Act
            await IntegrationTestsFixture.ExecuteServiceAsync<OrderingProcessManagerHandler>(sut =>
                sut.Handle(failedPaymentIntegrationEvent, CancellationToken.None));

            // Assert
            await IntegrationTestsFixture.ExecuteServiceAsync<SchedulerContext>(async dbContext =>
            {
                string messageType = ScheduledMessage.GetMessageTypeName(typeof(PaymentGracePeriodExpiredDomainEvent));
                MessageLog messageLog = await dbContext.MessageLogs
                    .OrderByDescending(ml => ml.DateCreated)
                    .FirstOrDefaultAsync(ml => ml.TypeName == messageType);
                messageLog.Should().NotBeNull();
                
                ScheduledMessage scheduledMessage = JsonConvert.DeserializeObject<ScheduledMessage>(messageLog!.Body);
                scheduledMessage.Should().NotBeNull();
                scheduledMessage!.CausationId.Should().Be(failedPaymentIntegrationEvent.MessageId);
            });
        }
        
        [Theory]
        [CustomizedAutoData]
        public async Task Unpaid_order_is_cancelled_after_payment_grace_period_expires
        (
            EntityId orderId,
            ShoppingCart shoppingCart
        )
        {
            // Arrange
            IClockService clockService = new ClockService();

            OrderingProcessManager processManager = await OrderHelper.PlaceOrderAsync(shoppingCart, orderId, clockService.Now);
            processManager.Transition(new PaymentFailedIntegrationEvent(orderId), clockService.Now);
            
            await OrderHelper.SaveAndPublishAsync(processManager);
            
            PaymentGracePeriodExpiredDomainEvent paymentGracePeriodExpired = new(orderId);
        
            // Act
            await IntegrationTestsFixture.ExecuteServiceAsync<OrderingProcessManagerHandler>(sut =>
                sut.Handle(paymentGracePeriodExpired, CancellationToken.None));
            
            // Assert
            Order orderFromDb = await OrderHelper.GetOrderAsync(orderId);
            orderFromDb.Should().NotBeNull();
            orderFromDb.Status.Should().Be(OrderStatus.Cancelled);
        }
        
        [Theory]
        [CustomizedAutoData]
        public async Task Paid_order_is_not_cancelled_after_payment_grace_period_expires
        (
            EntityId orderId,
            ShoppingCart shoppingCart
        )
        {
            // Arrange
            IClockService clockService = new ClockService();

            OrderingProcessManager processManager = await OrderHelper.PlaceOrderAsync(shoppingCart, orderId, clockService.Now);
            processManager.Transition(new PaymentSucceededIntegrationEvent(orderId), clockService.Now);
            
            await OrderHelper.SaveAndPublishAsync(processManager);
            
            PaymentGracePeriodExpiredDomainEvent paymentGracePeriodExpired = new(orderId);
        
            // Act
            await IntegrationTestsFixture.ExecuteServiceAsync<OrderingProcessManagerHandler>(sut =>
                sut.Handle(paymentGracePeriodExpired, CancellationToken.None));
            
            // Assert
            Order orderFromDb = await OrderHelper.GetOrderAsync(orderId);
            orderFromDb.Should().NotBeNull();
            orderFromDb.Status.Should().NotBe(OrderStatus.Cancelled);
        }
        
        [Theory]
        [CustomizedAutoData]
        public async Task Payment_success_schedules_a_reminder_message
        (
            EntityId orderId,
            ShoppingCart shoppingCart
        )
        {
            // Arrange
            IClockService clockService = new ClockService();

            await OrderHelper.PlaceOrderAsync(shoppingCart, orderId, clockService.Now);
        
            PaymentSucceededIntegrationEvent paymentSucceededIntegrationEvent = new(orderId);
        
            // Act
            await IntegrationTestsFixture.ExecuteServiceAsync<OrderingProcessManagerHandler>(sut =>
                sut.Handle(paymentSucceededIntegrationEvent, CancellationToken.None));
            
            // Assert
            await IntegrationTestsFixture.ExecuteServiceAsync<SchedulerContext>(async dbContext =>
            {
                string messageType = ScheduledMessage.GetMessageTypeName(typeof(ShippingGracePeriodExpiredDomainEvent));
                MessageLog messageLog = await dbContext.MessageLogs
                    .OrderByDescending(ml => ml.DateCreated)
                    .FirstOrDefaultAsync(ml => ml.TypeName == messageType);
                messageLog.Should().NotBeNull();

                ScheduledMessage scheduledMessage = JsonConvert.DeserializeObject<ScheduledMessage>(messageLog!.Body);
                scheduledMessage.Should().NotBeNull();
                scheduledMessage!.CausationId.Should().Be(paymentSucceededIntegrationEvent.MessageId);
            });
        }
        
        [Theory]
        [CustomizedAutoData]
        public async Task Order_pending_shipping_is_cancelled_after_too_many_shipping_check_tries
        (
            EntityId orderId,
            ShoppingCart shoppingCart
        )
        {
            // Arrange
            IClockService clockService = new ClockService();

            OrderingProcessManager processManager = await OrderHelper.PlaceOrderAsync(shoppingCart, orderId, clockService.Now);
            
            ShippingGracePeriodExpiredDomainEvent shippingGracePeriodExpiredDomainEvent = new 
            (
                orderId,
                new PaymentSucceededIntegrationEvent(orderId)
            );
            
            while (processManager.ShippingCheckCount < OrderingProcessManager.Settings.ShippingCheckThreshold - 1)
                processManager.Transition(shippingGracePeriodExpiredDomainEvent, clockService.Now);
            
            await OrderHelper.SaveAndPublishAsync(processManager);

            // Act
            await IntegrationTestsFixture.ExecuteServiceAsync<OrderingProcessManagerHandler>(sut =>
                sut.Handle(shippingGracePeriodExpiredDomainEvent, CancellationToken.None));
            
            // Assert
            Order orderFromDb = await OrderHelper.GetOrderAsync(orderId);
            orderFromDb.Should().NotBeNull();
            orderFromDb.Status.Should().Be(OrderStatus.Cancelled);
        }
        
        //[Fact]
        // public async Task Shipped_order_is_not_cancelled_after_shipping_grace_period_expires()
        // {
        //     // TODO - ShippingGracePeriodExpiredDomainEvent handler - check when status = OrderShipped
        //     // This can be addressed with development of the Shipping bounded context
        // }
        
        [Theory]
        [CustomizedAutoData]
        public async Task Payment_success_reminder_is_resent_after_shipping_grace_period_expires
        (
            EntityId orderId,
            ShoppingCart shoppingCart
        )
        {
            // Arrange
            IClockService clockService = new ClockService();

            OrderingProcessManager processManager = await OrderHelper.PlaceOrderAsync(shoppingCart, orderId, clockService.Now);
        
            ShippingGracePeriodExpiredDomainEvent shippingGracePeriodExpiredDomainEvent = new 
            (
                orderId,
                new PaymentSucceededIntegrationEvent(orderId)
            );
        
            // Act
            await IntegrationTestsFixture.ExecuteServiceAsync<OrderingProcessManagerHandler>(sut =>
                sut.Handle(shippingGracePeriodExpiredDomainEvent, CancellationToken.None));
            
            // Assert
            Order orderFromDb = await OrderHelper.GetOrderAsync(orderId);
            orderFromDb.Should().NotBeNull();
            orderFromDb.Status.Should().NotBe(OrderStatus.Cancelled);
            
            IReadOnlyList<IMessage> outboxMessages = await OrderHelper.GetProcessManagerOutboxAsync(processManager.Id);
            outboxMessages.OfType<PaymentSucceededIntegrationEvent>().Count().Should().Be(1);
        }
    }
}