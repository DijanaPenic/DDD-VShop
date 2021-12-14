using Xunit;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using AutoFixture;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

using VShop.SharedKernel.Tests;
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
using VShop.Modules.Sales.API.Application.Commands;
using VShop.Modules.Sales.API.Application.ProcessManagers;
using VShop.Modules.Sales.API.Tests.IntegrationTests.Helpers;
using VShop.Modules.Sales.API.Tests.IntegrationTests.Infrastructure;
using VShop.Modules.Billing.Integration.Events;

namespace VShop.Modules.Sales.API.Tests.IntegrationTests
{
    [Collection("Integration Tests Collection")]
    public class OrderingIntegrationTests : ResetDatabaseLifetime
    {
        private readonly Fixture _autoFixture;
        private readonly ShoppingCartHelper _shoppingCartHelper;
        private readonly OrderHelper _orderHelper;

        public OrderingIntegrationTests(AppFixture appFixture)
        {
            _autoFixture = appFixture.AutoFixture;
            _shoppingCartHelper = new ShoppingCartHelper(_autoFixture);
            _orderHelper = new OrderHelper(_autoFixture, _shoppingCartHelper);
        }

        [Fact]
        public async Task Shopping_cart_checkout_places_an_order()
        {
            // Arrange
            ShoppingCart shoppingCart = await _shoppingCartHelper.PrepareShoppingCartForCheckoutAsync();

            CheckoutShoppingCartCommand command = new(shoppingCart.Id);
            
            // Act
            Result<CheckoutOrder> result = await IntegrationTestsFixture.SendAsync(command);
            
            // Assert
            result.IsError(out _).Should().BeFalse();
            
            ShoppingCart shoppingCartFromDb = await _shoppingCartHelper.GetShoppingCartAsync(command.ShoppingCartId);
            shoppingCartFromDb.Status.Should().Be(ShoppingCartStatus.Closed); // The shopping cart should have been deleted.
            
            EntityId orderId = EntityId.Create(result.GetData().OrderId);

            Order orderFromDb = await _orderHelper.GetOrderAsync(orderId);
            orderFromDb.Should().NotBeNull(); // The order should have been created.
        }
        
        [Fact]
        public async Task Payment_failure_schedules_a_reminder_message()
        {
            // Arrange
            IClockService clockService = new ClockService();
            Guid orderId = _autoFixture.Create<Guid>();
            
            await _shoppingCartHelper.CheckoutShoppingCartAsync(clockService, orderId);
        
            PaymentFailedIntegrationEvent failedPaymentIntegrationEvent = new(orderId);
        
            // Act
            await IntegrationTestsFixture.ExecuteServiceAsync<OrderingProcessManagerHandler>(sut =>
                sut.Handle(failedPaymentIntegrationEvent, CancellationToken.None));

            // Assert
            await IntegrationTestsFixture.ExecuteServiceAsync<SchedulerContext>(async dbContext =>
            {
                MessageLog messageLog = await dbContext.MessageLogs.SingleOrDefaultAsync(ml =>
                    ml.TypeName == ScheduledMessage.GetMessageTypeName(typeof(PaymentGracePeriodExpiredDomainEvent)));
                messageLog.Should().NotBeNull();
            });
        }
        
        [Fact]
        public async Task Unpaid_order_is_cancelled_after_payment_grace_period_expires()
        {
            // Arrange
            IClockService clockService = new ClockService();
            Guid orderId = _autoFixture.Create<Guid>();
            
            OrderingProcessManager processManager = await _orderHelper.PlaceOrderAsync(clockService, orderId);
            processManager.Transition(new PaymentFailedIntegrationEvent(orderId));
            
            await _orderHelper.SaveProcessManagerAsync(processManager);
            
            PaymentGracePeriodExpiredDomainEvent paymentGracePeriodExpired = new(orderId);
        
            // Act
            await IntegrationTestsFixture.ExecuteServiceAsync<OrderingProcessManagerHandler>(sut =>
                sut.Handle(paymentGracePeriodExpired, CancellationToken.None));
            
            // Assert
            Order orderFromDb = await _orderHelper.GetOrderAsync(orderId);
            orderFromDb.Status.Should().Be(OrderStatus.Cancelled);
        }
        
        [Fact]
        public async Task Paid_order_is_not_cancelled_after_payment_grace_period_expires()
        {
            // Arrange
            IClockService clockService = new ClockService();
            Guid orderId = _autoFixture.Create<Guid>();
            
            OrderingProcessManager processManager = await _orderHelper.PlaceOrderAsync(clockService, orderId);
            processManager.Transition(new PaymentSucceededIntegrationEvent(orderId));
            
            await _orderHelper.SaveProcessManagerAsync(processManager);
            
            PaymentGracePeriodExpiredDomainEvent paymentGracePeriodExpired = new(orderId);
        
            // Act
            await IntegrationTestsFixture.ExecuteServiceAsync<OrderingProcessManagerHandler>(sut =>
                sut.Handle(paymentGracePeriodExpired, CancellationToken.None));
            
            // Assert
            Order orderFromDb = await _orderHelper.GetOrderAsync(orderId);
            orderFromDb.Status.Should().NotBe(OrderStatus.Cancelled);
        }
        
        [Fact]
        public async Task Payment_success_schedules_a_reminder_message()
        {
            // Arrange
            IClockService clockService = new ClockService();
            Guid orderId = _autoFixture.Create<Guid>();
            
            await _shoppingCartHelper.CheckoutShoppingCartAsync(clockService, orderId);
        
            PaymentSucceededIntegrationEvent paymentSucceededIntegrationEvent = new(orderId);
        
            // Act
            await IntegrationTestsFixture.ExecuteServiceAsync<OrderingProcessManagerHandler>(sut =>
                sut.Handle(paymentSucceededIntegrationEvent, CancellationToken.None));
            
            // Assert
            await IntegrationTestsFixture.ExecuteServiceAsync<SchedulerContext>(async dbContext =>
            {
                MessageLog messageLog = await dbContext.MessageLogs.SingleOrDefaultAsync(ml =>
                    ml.TypeName == ScheduledMessage.GetMessageTypeName(typeof(ShippingGracePeriodExpiredDomainEvent)));
                messageLog.Should().NotBeNull();
            });
        }
        
        [Fact]
        public async Task Order_pending_shipping_is_cancelled_after_too_many_shipping_check_tries()
        {
            // Arrange
            IClockService clockService = new ClockService();
            Guid orderId = _autoFixture.Create<Guid>();
            
            OrderingProcessManager processManager = await _orderHelper.PlaceOrderAsync(clockService, orderId);
            
            ShippingGracePeriodExpiredDomainEvent shippingGracePeriodExpiredDomainEvent = new 
            (
                orderId,
                new PaymentSucceededIntegrationEvent(orderId)
            );
            
            while (processManager.ShippingCheckCount < OrderingProcessManager.Settings.ShippingCheckThreshold)
                processManager.Transition(shippingGracePeriodExpiredDomainEvent);
            
            await _orderHelper.SaveProcessManagerAsync(processManager);
        
            // Act
            await IntegrationTestsFixture.ExecuteServiceAsync<OrderingProcessManagerHandler>(sut =>
                sut.Handle(shippingGracePeriodExpiredDomainEvent, CancellationToken.None));
            
            // Assert
            Order orderFromDb = await _orderHelper.GetOrderAsync(orderId);
            orderFromDb.Status.Should().Be(OrderStatus.Cancelled);
        }
        
        //[Fact]
        // public async Task Shipped_order_is_not_cancelled_after_shipping_grace_period_expires()
        // {
        //     // TODO - ShippingGracePeriodExpiredDomainEvent handler - check when status = OrderShipped
        //     // This can be addressed with development of the Shipping bounded context
        // }
        
        [Fact]
        public async Task Payment_success_reminder_is_resent_after_shipping_grace_period_expires()
        {
            // Arrange
            IClockService clockService = new ClockService();
            Guid orderId = _autoFixture.Create<Guid>();
            
            OrderingProcessManager processManager = await _orderHelper.PlaceOrderAsync(clockService, orderId);
        
            ShippingGracePeriodExpiredDomainEvent shippingGracePeriodExpiredDomainEvent = new 
            (
                orderId,
                new PaymentSucceededIntegrationEvent(orderId)
            );
        
            // Act
            await IntegrationTestsFixture.ExecuteServiceAsync<OrderingProcessManagerHandler>(sut =>
                sut.Handle(shippingGracePeriodExpiredDomainEvent, CancellationToken.None));
            
            // Assert
            Order orderFromDb = await _orderHelper.GetOrderAsync(orderId);
            orderFromDb.Status.Should().NotBe(OrderStatus.Cancelled);
            
            IList<IMessage> outboxMessages = await _orderHelper.GetProcessManagerOutboxAsync(processManager.Id);
            outboxMessages.OfType<PaymentSucceededIntegrationEvent>().Count().Should().Be(1);
        }
    }
}