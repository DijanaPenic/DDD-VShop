using Xunit;
using FluentAssertions;

using VShop.Modules.Billing.Integration.Events;
using VShop.Modules.ProcessManager.Infrastructure.ProcessManagers.Ordering;
using VShop.Modules.Sales.Domain.Models.ShoppingCart;
using VShop.SharedKernel.Domain.ValueObjects;
using VShop.SharedKernel.EventSourcing.Stores.Contracts;
using VShop.SharedKernel.Infrastructure.Contexts.Contracts;
using VShop.SharedKernel.Infrastructure.Events.Contracts;
using VShop.SharedKernel.Infrastructure.Messaging;
using VShop.SharedKernel.Infrastructure.Services;
using VShop.SharedKernel.Infrastructure.Services.Contracts;
using VShop.SharedKernel.Integration.Stores.Contracts;
using VShop.SharedKernel.Tests.Customizations;
using VShop.SharedKernel.Tests.IntegrationTests.Probing;
using VShop.Tests.IntegrationTests.Helpers;
using VShop.Tests.IntegrationTests.Infrastructure;

namespace VShop.Tests.IntegrationTests.EventSubscriptions
{
    [Collection("Non-Parallel Tests Collection")]
    public class IntegrationEventProjectionIntegrationTests : TestBase, IClassFixture<SubscriptionFixture>
    {
        private const int TimeoutInMillis = 4000;

        [Theory(Skip = "The 'Ordering' process manager doesn't issue any integration events, so can't test.")]
        [CustomAutoData]
        internal async Task Projecting_integration_event_from_process_manager_stream_into_the_integration_stream
        (
            EntityId orderId,
            ShoppingCart shoppingCart
        )
        {
            // Arrange
            IClockService clockService = new ClockService();
        
            await OrderHelper.PlaceOrderAsync(shoppingCart, orderId, clockService.Now);
            OrderingProcessManager processManager = await ProcessManagerHelper.GetOrderingProcessManagerAsync(orderId);

            PaymentSucceededIntegrationEvent paymentSucceededIntegrationEvent = new(orderId);

            // Act
            // await IntegrationTestsFixture.ExecuteServiceAsync<CustomEventStoreClient>
            // (
            //     eventStoreClient => eventStoreClient.AppendToStreamAsync
            //     (
            //         ProcessManagerStore<OrderingProcessManager>.GetOutboxStreamName(processManager.Id),
            //         processManager.Outbox.Version,
            //         new List<IMessage> { paymentSucceededIntegrationEvent },
            //         CancellationToken.None
            //     )
            // );
            
            // Assert
            // async Task<IReadOnlyList<IIntegrationEvent>> Sampling(IIntegrationEventStore store) 
            //     => (await store.LoadAsync(CancellationToken.None)).ToMessages();
            //
            // void Validation(IReadOnlyList<IIntegrationEvent> integrationEvents)
            //     => integrationEvents.OfType<PaymentSucceededIntegrationEvent>().SingleOrDefault().Should().NotBeNull();
            //
            // await IntegrationTestsFixture.AssertEventuallyAsync
            // (
            //     clockService,
            //     new PostgresDatabaseProbe<IIntegrationEventStore, IReadOnlyList<IIntegrationEvent>>(Sampling, Validation),
            //     TimeoutInMillis
            // );
        }
        
        [Theory, CustomAutoData]
        internal async Task Publishing_integration_event_from_the_integration_stream
        (
            EntityId orderId,
            ShoppingCart shoppingCart,
            IContext context
        )
        {
            // Arrange
            IClockService clockService = new ClockService();

            await OrderHelper.PlaceOrderAsync(shoppingCart, orderId, clockService.Now);
            
            PaymentSucceededIntegrationEvent paymentSucceededIntegrationEvent = new(orderId);
            
            // Act
            await ProcessManagerModule.ExecuteServiceAsync<IIntegrationEventStore>(store =>
                store.SaveAsync
                (
                    new MessageEnvelope<IIntegrationEvent>
                    (
                        paymentSucceededIntegrationEvent,
                        new MessageContext(context)
                    ),
                    CancellationToken.None
                ));
            
            // Assert
            Task<OrderingProcessManager> Sampling(IProcessManagerStore<OrderingProcessManager> store)
                => store.LoadAsync(orderId);

            void Validation(OrderingProcessManager processManager)
            {
                processManager.Should().NotBeNull();
                processManager.Status.Should().Be(OrderingProcessManagerStatus.OrderPaymentSucceeded);
            }

            // TODO - potentially refactor.
            await ProcessManagerModule.AssertEventuallyAsync
            (
                clockService,
                new PostgresDatabaseProbe<IProcessManagerStore<OrderingProcessManager>, OrderingProcessManager>
                    (ProcessManagerModule, Sampling, Validation),
                TimeoutInMillis
            );
        }
    }
}