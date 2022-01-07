using Xunit;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using FluentAssertions;
using EventStore.Client;

using VShop.SharedKernel.Messaging;
using VShop.SharedKernel.Messaging.Events;
using VShop.SharedKernel.Domain.ValueObjects;
using VShop.SharedKernel.EventSourcing.Stores;
using VShop.SharedKernel.Infrastructure.Services;
using VShop.SharedKernel.Infrastructure.Services.Contracts;
using VShop.SharedKernel.Integration.Stores.Contracts;
using VShop.SharedKernel.EventSourcing.Stores.Contracts;
using VShop.SharedKernel.EventStoreDb.Extensions;
using VShop.Modules.Billing.Integration.Events;
using VShop.Modules.Sales.Tests.Customizations;
using VShop.Modules.Sales.Domain.Models.ShoppingCart;
using VShop.Modules.Sales.API.Application.ProcessManagers;
using VShop.Modules.Sales.API.Tests.IntegrationTests.Helpers;
using VShop.Modules.Sales.API.Tests.IntegrationTests.Infrastructure;

namespace VShop.Modules.Sales.API.Tests.IntegrationTests
{
    [Collection("Non-Parallel Tests Collection")]
    public class IntegrationEventProjectionIntegrationTests : ResetDatabaseLifetime, IClassFixture<SubscriptionFixture>
    {
        private const int TimeoutInMillis = 4000;

        [Theory]
        [CustomizedAutoData]
        public async Task Projecting_integration_event_from_process_manager_stream_into_the_integration_stream
        (
            EntityId orderId,
            ShoppingCart shoppingCart,
            Guid causationId,
            Guid correlationId
        )
        {
            // Arrange
            IClockService clockService = new ClockService();
        
            await OrderHelper.PlaceOrderAsync(shoppingCart, orderId, clockService.Now);
            
            PaymentSucceededIntegrationEvent paymentSucceededIntegrationEvent = new(orderId);

            OrderingProcessManager processManager = await OrderHelper.LoadProcessManagerAsync
            (
                orderId,
                causationId,
                correlationId
            );
        
            // Act
            await IntegrationTestsFixture.ExecuteServiceAsync<EventStoreClient>
            (
                eventStoreClient => eventStoreClient.AppendToStreamAsync
                (
                    ProcessManagerStore<OrderingProcessManager>.GetOutboxStreamName(processManager.Id),
                    processManager.Outbox.Version,
                    new List<IMessage> { paymentSucceededIntegrationEvent },
                    clockService.Now,
                    CancellationToken.None
                )
            );
        
            // Assert
            async Task<IReadOnlyList<IIntegrationEvent>> Sampling(IIntegrationEventStore store) 
                => await store.LoadAsync(CancellationToken.None);
        
            void Validation(IReadOnlyList<IIntegrationEvent> integrationEvents)
                => integrationEvents.OfType<PaymentSucceededIntegrationEvent>().SingleOrDefault().Should().NotBeNull();
        
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
            EntityId orderId,
            ShoppingCart shoppingCart,
            Guid causationId
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
                => store.LoadAsync(orderId, causationId);

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