// using Xunit;
// using FluentAssertions;
//
// using VShop.Modules.Billing.Integration.Events;
// using VShop.Modules.Sales.Domain.Models.ShoppingCart;
// using VShop.Modules.Sales.Infrastructure.ProcessManagers;
// using VShop.Modules.Sales.Tests.Customizations;
// using VShop.Modules.Sales.Tests.IntegrationTests.Helpers;
// using VShop.Modules.Sales.Tests.IntegrationTests.Infrastructure;
// using VShop.SharedKernel.Domain.ValueObjects;
// using VShop.SharedKernel.EventSourcing.Stores.Contracts;
// using VShop.SharedKernel.Infrastructure.Contexts.Contracts;
// using VShop.SharedKernel.Infrastructure.Events.Contracts;
// using VShop.SharedKernel.Infrastructure.Messaging;
// using VShop.SharedKernel.Infrastructure.Services;
// using VShop.SharedKernel.Infrastructure.Services.Contracts;
// using VShop.SharedKernel.Integration.Stores.Contracts;
//
// namespace VShop.Modules.Sales.Tests.IntegrationTests
// {
//     [Collection("Non-Parallel Tests Collection")]
//     public class IntegrationEventProjectionIntegrationTests : ResetDatabaseLifetime, IClassFixture<SubscriptionFixture>
//     {
//         private const int TimeoutInMillis = 4000;
//
//         [CustomizedAutoData]
//         [Theory(Skip = "The 'Ordering' process manager doesn't issue any integration events, so can't test.")]
//         internal async Task Projecting_integration_event_from_process_manager_stream_into_the_integration_stream
//         (
//             EntityId orderId,
//             ShoppingCart shoppingCart
//         )
//         {
//             // Arrange
//             IClockService clockService = new ClockService();
//         
//             await OrderHelper.PlaceOrderAsync(shoppingCart, orderId, clockService.Now);
//             OrderingProcessManager processManager = await OrderHelper.LoadProcessManagerAsync(orderId);
//
//             PaymentSucceededIntegrationEvent paymentSucceededIntegrationEvent = new(orderId);
//
//             // Act
//             // await IntegrationTestsFixture.ExecuteServiceAsync<CustomEventStoreClient>
//             // (
//             //     eventStoreClient => eventStoreClient.AppendToStreamAsync
//             //     (
//             //         ProcessManagerStore<OrderingProcessManager>.GetOutboxStreamName(processManager.Id),
//             //         processManager.Outbox.Version,
//             //         new List<IMessage> { paymentSucceededIntegrationEvent },
//             //         CancellationToken.None
//             //     )
//             // );
//             
//             // Assert
//             async Task<IReadOnlyList<IIntegrationEvent>> Sampling(IIntegrationEventStore store) 
//                 => (await store.LoadAsync(CancellationToken.None)).ToMessages();
//         
//             void Validation(IReadOnlyList<IIntegrationEvent> integrationEvents)
//                 => integrationEvents.OfType<PaymentSucceededIntegrationEvent>().SingleOrDefault().Should().NotBeNull();
//         
//             await IntegrationTestsFixture.AssertEventuallyAsync
//             (
//                 clockService,
//                 new DatabaseProbe<IIntegrationEventStore, IReadOnlyList<IIntegrationEvent>>(Sampling, Validation),
//                 TimeoutInMillis
//             );
//         }
//         
//         [Theory]
//         [CustomizedAutoData]
//         internal async Task Publishing_integration_event_from_the_integration_stream
//         (
//             EntityId orderId,
//             ShoppingCart shoppingCart,
//             IContext context
//         )
//         {
//             // Arrange
//             IClockService clockService = new ClockService();
//
//             await OrderHelper.PlaceOrderAsync(shoppingCart, orderId, clockService.Now);
//             
//             PaymentSucceededIntegrationEvent paymentSucceededIntegrationEvent = new(orderId);
//             
//             // Act
//             await IntegrationTestsFixture.ExecuteServiceAsync<IIntegrationEventStore>(store =>
//                 store.SaveAsync
//                 (
//                     new MessageEnvelope<IIntegrationEvent>
//                     (
//                         paymentSucceededIntegrationEvent,
//                         new MessageContext(context)
//                     ),
//                     CancellationToken.None
//                 ));
//             
//             // Assert
//             Task<OrderingProcessManager> Sampling(IProcessManagerStore<OrderingProcessManager> store)
//                 => store.LoadAsync(orderId);
//
//             void Validation(OrderingProcessManager processManager)
//             {
//                 processManager.Should().NotBeNull();
//                 processManager.Status.Should().Be(OrderingProcessManagerStatus.OrderPaymentSucceeded);
//             }
//
//             await IntegrationTestsFixture.AssertEventuallyAsync
//             (
//                 clockService,
//                 new DatabaseProbe<IProcessManagerStore<OrderingProcessManager>, OrderingProcessManager>(Sampling, Validation),
//                 TimeoutInMillis
//             );
//         }
//     }
// }