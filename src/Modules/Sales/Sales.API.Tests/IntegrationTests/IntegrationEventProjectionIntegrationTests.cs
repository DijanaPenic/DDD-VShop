using Xunit;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using FluentAssertions;

using VShop.Modules.Sales.Domain.Events;
using VShop.Modules.Sales.Domain.Models.ShoppingCart;
using VShop.Modules.Sales.Tests.Customizations;
using VShop.Modules.Sales.API.Application.ProcessManagers;
using VShop.Modules.Sales.API.Tests.IntegrationTests.Helpers;
using VShop.Modules.Sales.API.Tests.IntegrationTests.Infrastructure;
using VShop.Modules.Billing.Integration.Events;
using VShop.SharedKernel.Messaging.Events;
using VShop.SharedKernel.Infrastructure.Services;
using VShop.SharedKernel.Infrastructure.Services.Contracts;
using VShop.SharedKernel.Integration.Stores.Contracts;
using VShop.SharedKernel.EventSourcing.Stores.Contracts;

namespace VShop.Modules.Sales.API.Tests.IntegrationTests
{
    [Collection("Non-Parallel Tests Collection")]
    public class IntegrationEventProjectionIntegrationTests : ResetDatabaseLifetime, IClassFixture<SubscriptionFixture>
    {
        private const int TimeoutInMillis = 3000;
            
        [Theory]
        [CustomizedAutoData]
        public async Task Projecting_integration_event_from_process_manager_stream_into_the_integration_stream
        (
            Guid orderId,
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
            
            processManager.Transition(shippingGracePeriodExpiredDomainEvent, clockService.Now);

            // Act
            await OrderHelper.SaveAsync(processManager);
        
            // Assert
            async Task<IReadOnlyList<IIntegrationEvent>> Sampling(IIntegrationEventStore store) 
                => (await store.LoadAsync(CancellationToken.None)).ToList();

            void Validation(IReadOnlyList<IIntegrationEvent> integrationEvents)
            {
                integrationEvents.Count.Should().Be(1);
                integrationEvents.SingleOrDefault().Should().BeOfType<PaymentSucceededIntegrationEvent>();
            }

            await IntegrationTestsFixture.AssertEventuallyAsync
            (
                clockService,
                new DatabaseProbe<IIntegrationEventStore, IReadOnlyList<IIntegrationEvent>>(Sampling, Validation),
                TimeoutInMillis
            );
        }
        
        [Theory]
        [CustomizedAutoData]
        public async Task Publishing_integration_event_from_the_integration_stream
        (
            Guid orderId,
            ShoppingCart shoppingCart
        )
        {
            // Arrange
            IClockService clockService = new ClockService();

            await OrderHelper.PlaceOrderAsync(shoppingCart, orderId, clockService.Now);
            
            PaymentSucceededIntegrationEvent paymentSucceededIntegrationEvent = new(orderId);
            
            // Act
            await IntegrationTestsFixture.ExecuteServiceAsync<IIntegrationEventStore>(store =>
                store.SaveAsync(paymentSucceededIntegrationEvent, CancellationToken.None));
            
            // Assert
            Task<OrderingProcessManager> Sampling(IProcessManagerStore<OrderingProcessManager> store)
                => store.LoadAsync(orderId);

            void Validation(OrderingProcessManager processManager)
            {
                processManager.Should().NotBeNull();
                processManager.Status.Should().Be(OrderingProcessManagerStatus.OrderPaymentSucceeded);
            }

            await IntegrationTestsFixture.AssertEventuallyAsync
            (
                clockService,
                new DatabaseProbe<IProcessManagerStore<OrderingProcessManager>, OrderingProcessManager>(Sampling, Validation),
                TimeoutInMillis
            );
        }
    }
}