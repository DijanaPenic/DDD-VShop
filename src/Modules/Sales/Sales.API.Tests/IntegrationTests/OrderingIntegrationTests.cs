using Xunit;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Serilog;
using AutoFixture;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

using VShop.SharedKernel.Tests;
using VShop.SharedKernel.Messaging;
using VShop.SharedKernel.Infrastructure;
using VShop.SharedKernel.Infrastructure.Services.Contracts;
using VShop.SharedKernel.Domain.ValueObjects;
using VShop.SharedKernel.Scheduler.Infrastructure.Entities;
using VShop.SharedKernel.EventSourcing.Repositories.Contracts;
using VShop.Modules.Sales.Domain.Enums;
using VShop.Modules.Sales.Domain.Events;
using VShop.Modules.Sales.Domain.Models.Ordering;
using VShop.Modules.Sales.Domain.Models.ShoppingCart;
using VShop.Modules.Billing.Integration.Events;
using VShop.Modules.Sales.API.Application.Commands;
using VShop.Modules.Sales.API.Application.ProcessManagers;

using SchedulerContext = VShop.SharedKernel.Scheduler.Infrastructure.SchedulerContext;

namespace VShop.Modules.Sales.API.Tests.IntegrationTests
{
    [Collection("Integration Tests Collection")]
    public class OrderingIntegrationTests : IntegrationTestsBase
    {
        private readonly Fixture _autoFixture;

        public OrderingIntegrationTests(AppFixture appFixture) => _autoFixture = appFixture.AutoFixture;

        [Fact]
        public async Task Shopping_cart_checkout_places_an_order()
        {
            // Arrange
            IAggregateRepository<ShoppingCart, EntityId> shoppingCartRepository = 
                GetService<IAggregateRepository<ShoppingCart, EntityId>>();
            IAggregateRepository<Order, EntityId> orderRepository = 
                GetService<IAggregateRepository<Order, EntityId>>();
            IProcessManagerRepository<OrderingProcessManager> processManagerRepository = 
                GetService<IProcessManagerRepository<OrderingProcessManager>>();
            IClockService clockService = GetService<IClockService>();
            
            CheckoutShoppingCartCommandHandler sut = new(clockService, shoppingCartRepository);

            ShoppingCart shoppingCart = ShoppingCartFixture.GetShoppingCartForCheckoutFixture(_autoFixture);
            await shoppingCartRepository.SaveAsync(shoppingCart, CancellationToken.None);

            CheckoutShoppingCartCommand command = new(shoppingCart.Id);
            
            // Act
            Result<CheckoutOrder> result = await sut.Handle(command, CancellationToken.None);
            
            // Assert
            result.IsError(out _).Should().BeFalse();
            
            ShoppingCart shoppingCartFromDb = await shoppingCartRepository.LoadAsync(EntityId.Create(command.ShoppingCartId));
            shoppingCartFromDb.Status.Should().Be(ShoppingCartStatus.Closed); // The shopping cart should have been deleted.
            
            EntityId orderId = EntityId.Create(result.GetData().OrderId);
            
            Order orderFromDb = await orderRepository.LoadAsync(orderId);
            orderFromDb.Should().NotBeNull(); // The order should have been created.

            OrderingProcessManager processManagerFromDb = await processManagerRepository.LoadAsync(orderId);
            processManagerFromDb.Should().NotBeNull(); // The process manager should have been created.
        }
        
        [Fact]
        public async Task Payment_failure_triggers_a_reminder_message()
        {
            // Arrange
            SchedulerContext schedulerContext = GetService<SchedulerContext>();
            IScheduler scheduler = await GetService<ISchedulerFactory>().GetScheduler();
            ILogger logger = GetService<ILogger>();
            IClockService clockService = GetService<IClockService>();
            IProcessManagerRepository<OrderingProcessManager> processManagerRepository = 
                GetService<IProcessManagerRepository<OrderingProcessManager>>();
            IAggregateRepository<ShoppingCart, EntityId> shoppingCartRepository = 
                GetService<IAggregateRepository<ShoppingCart, EntityId>>();

            OrderingProcessManagerHandler sut = new(logger, processManagerRepository);

            Guid orderId = _autoFixture.Create<Guid>();
            
            ShoppingCart shoppingCart = ShoppingCartFixture.GetShoppingCartForCheckoutFixture(_autoFixture);
            shoppingCart.RequestCheckout(clockService, EntityId.Create(orderId));
            
            await shoppingCartRepository.SaveAsync(shoppingCart);

            PaymentFailedIntegrationEvent failedPaymentIntegrationEvent = new(orderId);

            // Act
            await sut.Handle(failedPaymentIntegrationEvent, CancellationToken.None);
            
            // Assert
            MessageLog messageLog = await schedulerContext.MessageLogs.SingleOrDefaultAsync(ml =>
                ml.TypeName == ScheduledMessage.GetMessageTypeName(typeof(PaymentGracePeriodExpiredDomainEvent)));
            messageLog.Should().NotBeNull();
            
            IJobDetail job = await scheduler.GetJobDetail(new JobKey(messageLog!.Id.ToString()));
            job.Should().NotBeNull();

            IReadOnlyCollection<ITrigger> triggers = await scheduler.GetTriggersOfJob(job!.Key);
            triggers.Should().HaveCount(1);
        }
        
        [Fact]
        public async Task Unpaid_order_is_cancelled_after_payment_grace_period_expires()
        {
            // Arrange
            ILogger logger = GetService<ILogger>();
            IProcessManagerRepository<OrderingProcessManager> processManagerRepository = 
                GetService<IProcessManagerRepository<OrderingProcessManager>>();
            IAggregateRepository<Order, EntityId> orderRepository = 
                GetService<IAggregateRepository<Order, EntityId>>();

            OrderingProcessManagerHandler sut = new(logger, processManagerRepository);

            OrderingProcessManager processManager = await PlacedOrderAsync();
            processManager.Transition(new PaymentFailedIntegrationEvent(processManager.Id));
            
            await processManagerRepository.SaveAsync(processManager);
            
            PaymentGracePeriodExpiredDomainEvent paymentGracePeriodExpired = new(processManager.Id);

            // Act
            await sut.Handle(paymentGracePeriodExpired, CancellationToken.None);
            
            // Assert
            Order orderFromDb = await orderRepository.LoadAsync(EntityId.Create(processManager.Id));
            orderFromDb.Status.Should().Be(OrderStatus.Cancelled);
        }
        
        [Fact]
        public async Task Paid_order_is_not_cancelled_after_payment_grace_period_expires()
        {
            // Arrange
            ILogger logger = GetService<ILogger>();
            IProcessManagerRepository<OrderingProcessManager> processManagerRepository = 
                GetService<IProcessManagerRepository<OrderingProcessManager>>();
            IAggregateRepository<Order, EntityId> orderRepository = 
                GetService<IAggregateRepository<Order, EntityId>>();

            OrderingProcessManagerHandler sut = new(logger, processManagerRepository);

            OrderingProcessManager processManager = await PlacedOrderAsync();
            processManager.Transition(new PaymentFailedIntegrationEvent(processManager.Id));
            processManager.Transition(new PaymentSucceededIntegrationEvent(processManager.Id));
            
            await processManagerRepository.SaveAsync(processManager);
            
            PaymentGracePeriodExpiredDomainEvent paymentGracePeriodExpired = new(processManager.Id);

            // Act
            await sut.Handle(paymentGracePeriodExpired, CancellationToken.None);
            
            // Assert
            Order orderFromDb = await orderRepository.LoadAsync(EntityId.Create(processManager.Id));
            orderFromDb.Status.Should().NotBe(OrderStatus.Cancelled);
        }
        
        [Fact]
        public async Task Payment_success_triggers_a_reminder_message()
        {
            // Arrange
            SchedulerContext schedulerContext = GetService<SchedulerContext>();
            IScheduler scheduler = await GetService<ISchedulerFactory>().GetScheduler();
            ILogger logger = GetService<ILogger>();
            IClockService clockService = GetService<IClockService>();
            IProcessManagerRepository<OrderingProcessManager> processManagerRepository = 
                GetService<IProcessManagerRepository<OrderingProcessManager>>();
            IAggregateRepository<ShoppingCart, EntityId> shoppingCartRepository = 
                GetService<IAggregateRepository<ShoppingCart, EntityId>>();

            OrderingProcessManagerHandler sut = new(logger, processManagerRepository);

            Guid orderId = _autoFixture.Create<Guid>();
            
            ShoppingCart shoppingCart = ShoppingCartFixture.GetShoppingCartForCheckoutFixture(_autoFixture);
            shoppingCart.RequestCheckout(clockService, EntityId.Create(orderId));
            
            await shoppingCartRepository.SaveAsync(shoppingCart);

            PaymentSucceededIntegrationEvent paymentSucceededIntegrationEvent = new(orderId);

            // Act
            await sut.Handle(paymentSucceededIntegrationEvent, CancellationToken.None);
            
            // Assert
            MessageLog messageLog = await schedulerContext.MessageLogs.SingleOrDefaultAsync(ml =>
                ml.TypeName == ScheduledMessage.GetMessageTypeName(typeof(ShippingGracePeriodExpiredDomainEvent)));
            messageLog.Should().NotBeNull();
            
            IJobDetail job = await scheduler.GetJobDetail(new JobKey(messageLog!.Id.ToString()));
            job.Should().NotBeNull();

            IReadOnlyCollection<ITrigger> triggers = await scheduler.GetTriggersOfJob(job!.Key);
            triggers.Should().HaveCount(1);
        }
        
        [Fact]
        public async Task Order_pending_shipping_is_cancelled_after_too_many_shipping_check_tries()
        {
            // Arrange
            ILogger logger = GetService<ILogger>();
            IProcessManagerRepository<OrderingProcessManager> processManagerRepository = 
                GetService<IProcessManagerRepository<OrderingProcessManager>>();
            IAggregateRepository<Order, EntityId> orderRepository = 
                GetService<IAggregateRepository<Order, EntityId>>();

            OrderingProcessManagerHandler sut = new(logger, processManagerRepository);
            
            OrderingProcessManager processManager = await PlacedOrderAsync();
            
            ShippingGracePeriodExpiredDomainEvent shippingGracePeriodExpiredDomainEvent = new 
            (
                processManager.Id,
                new PaymentSucceededIntegrationEvent(processManager.Id)
            );
            
            while (processManager.ShippingCheckCount < OrderingProcessManager.Settings.ShippingCheckThreshold)
                processManager.Transition(shippingGracePeriodExpiredDomainEvent);
            
            await processManagerRepository.SaveAsync(processManager);

            // Act
            await sut.Handle(shippingGracePeriodExpiredDomainEvent, CancellationToken.None);
            
            // Assert
            Order orderFromDb = await orderRepository.LoadAsync(EntityId.Create(processManager.Id));
            orderFromDb.Status.Should().Be(OrderStatus.Cancelled);
        }
        
        //[Fact]
        // public async Task Shipped_order_is_not_cancelled_after_shipping_grace_period_expires()
        // {
        //     // TODO - ShippingGracePeriodExpiredDomainEvent handler - check when status = OrderShipped
        //     // This can be addressed with development of the Shipping bounded context
        // }

        [Fact]
        public async Task Payment_success_reminder_is_sent_after_shipping_grace_period_expires()
        {
            // Arrange
            ILogger logger = GetService<ILogger>();
            IProcessManagerRepository<OrderingProcessManager> processManagerRepository = 
                GetService<IProcessManagerRepository<OrderingProcessManager>>();
            IAggregateRepository<Order, EntityId> orderRepository = 
                GetService<IAggregateRepository<Order, EntityId>>();

            OrderingProcessManagerHandler sut = new(logger, processManagerRepository);
            
            OrderingProcessManager processManager = await PlacedOrderAsync();

            ShippingGracePeriodExpiredDomainEvent shippingGracePeriodExpiredDomainEvent = new 
            (
                processManager.Id,
                new PaymentSucceededIntegrationEvent(processManager.Id)
            );

            // Act
            await sut.Handle(shippingGracePeriodExpiredDomainEvent, CancellationToken.None);
            
            // Assert
            Order orderFromDb = await orderRepository.LoadAsync(EntityId.Create(processManager.Id));
            orderFromDb.Status.Should().NotBe(OrderStatus.Cancelled);
            
            IList<IMessage> outboxMessages = await processManagerRepository.LoadOutboxAsync(processManager.Id);
            outboxMessages.OfType<PaymentSucceededIntegrationEvent>().Count().Should().Be(1);
        }

        private async Task<OrderingProcessManager> PlacedOrderAsync()
        {
            IClockService clockService = GetService<IClockService>();
            IProcessManagerRepository<OrderingProcessManager> processManagerRepository = 
                GetService<IProcessManagerRepository<OrderingProcessManager>>();
            IAggregateRepository<ShoppingCart, EntityId> shoppingCartRepository = 
                GetService<IAggregateRepository<ShoppingCart, EntityId>>();

            Guid orderId = _autoFixture.Create<Guid>();
            
            ShoppingCart shoppingCart = ShoppingCartFixture.GetShoppingCartForCheckoutFixture(_autoFixture);
            shoppingCart.RequestCheckout(clockService, EntityId.Create(orderId));
            
            await shoppingCartRepository.SaveAsync(shoppingCart);

            OrderingProcessManager processManager = await processManagerRepository.LoadAsync(orderId);

            return processManager;
        }
    }
}