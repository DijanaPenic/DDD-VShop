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
using VShop.SharedKernel.Messaging.Events;
using VShop.SharedKernel.Infrastructure.Services;
using VShop.SharedKernel.Infrastructure.Services.Contracts;
using VShop.SharedKernel.Integration.Repositories.Contracts;
using VShop.SharedKernel.EventSourcing.Repositories.Contracts;
using VShop.Modules.Billing.Integration.Events;

namespace VShop.Modules.Sales.API.Tests.IntegrationTests
{
    [Collection("Non-Parallel Tests Collection")]
    public class IntegrationEventProjectionIntegrationTests : ResetDatabaseLifetime, IClassFixture<SubscriptionFixture>
    {
        private const int TimeoutMillis = 3000;
            
        [Theory]
        [CustomizedAutoData]
        public async Task Projecting_PaymentSucceededIntegrationEvent_from_process_manager_stream_to_integration_stream
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
            
            processManager.Transition(shippingGracePeriodExpiredDomainEvent);

            // Act
            await OrderHelper.SaveAsync(processManager);
        
            // Assert
            async Task<IList<IIntegrationEvent>> Sampling(IIntegrationEventRepository integrationEventRepository) 
                => (await integrationEventRepository.LoadAsync(CancellationToken.None)).ToList();

            void Validation(IList<IIntegrationEvent> integrationEvents)
            {
                integrationEvents.Count.Should().Be(1);
                integrationEvents.FirstOrDefault().Should().BeOfType<PaymentSucceededIntegrationEvent>();
            }

            await IntegrationTestsFixture.AssertEventuallyAsync
            (
                clockService,
                new DatabaseProbe<IIntegrationEventRepository, IList<IIntegrationEvent>>(Sampling, Validation),
                TimeoutMillis
            );
        }
        
        [Theory]
        [CustomizedAutoData]
        public async Task Publishing_PaymentSucceededIntegrationEvent
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
            await IntegrationTestsFixture.ExecuteServiceAsync<IIntegrationEventRepository>(integrationEventRepository =>
                integrationEventRepository.SaveAsync(paymentSucceededIntegrationEvent, CancellationToken.None));
            
            // Assert
            Task<OrderingProcessManager> Sampling(IProcessManagerRepository<OrderingProcessManager> processManagerRepository)
                => processManagerRepository.LoadAsync(orderId);

            void Validation(OrderingProcessManager processManager)
            {
                processManager.Should().NotBeNull();
                processManager.Status.Should().Be(OrderingProcessManagerStatus.OrderPaymentSucceeded);
            }

            await IntegrationTestsFixture.AssertEventuallyAsync
            (
                clockService,
                new DatabaseProbe<IProcessManagerRepository<OrderingProcessManager>, OrderingProcessManager>(Sampling, Validation),
                TimeoutMillis
            );
        }
    }
}