// using Xunit;
// using System.Linq;
// using System.Threading.Tasks;
// using Microsoft.EntityFrameworkCore;
//
// using VShop.SharedKernel.Messaging;
// using VShop.SharedKernel.Domain.ValueObjects;
// using VShop.SharedKernel.Scheduler.Infrastructure;
// using VShop.SharedKernel.Scheduler.Infrastructure.Entities;
// using VShop.SharedKernel.Scheduler.Services.Contracts;
// using VShop.SharedKernel.Infrastructure.Services;
// using VShop.SharedKernel.Infrastructure.Services.Contracts;
// using VShop.SharedKernel.Tests.IntegrationTests.Probing;
// using VShop.Modules.Sales.Domain.Events;
// using VShop.Modules.Sales.Domain.Models.ShoppingCart;
// using VShop.Modules.Sales.Tests.Customizations;
// using VShop.Modules.Sales.API.Application.Commands;
// using VShop.Modules.Sales.API.Tests.IntegrationTests.Helpers;
// using VShop.Modules.Sales.API.Tests.IntegrationTests.Infrastructure;
//
// namespace VShop.Modules.Sales.API.Tests.IntegrationTests
// {
//     [Collection("Non-Parallel Tests Collection")]
//     public class SchedulerIntegrationTests : ResetDatabaseLifetime, IClassFixture<SchedulerFixture>
//     {
//         [Theory]
//         [CustomizedAutoData]
//         public async Task Scheduled_command_is_published_in_defined_time(ShoppingCart shoppingCart)
//         {
//             // Arrange
//             IClockService clockService = new ClockService();
//
//             await ShoppingCartHelper.SaveAsync(shoppingCart);
//         
//             IScheduledMessage scheduledMessage = new ScheduledMessage
//             (
//                 new DeleteShoppingCartCommand(shoppingCart.Id),
//                 clockService.Now
//             );
//         
//             // Act
//             await IntegrationTestsFixture.ExecuteServiceAsync<ISchedulerService>(sut =>
//                 sut.ScheduleMessageAsync(scheduledMessage));
//         
//             // Assert
//             await IntegrationTestsFixture.AssertEventuallyAsync
//             (
//                 clockService,
//                 new GetScheduledMessageStatusProbe(),
//                 3000
//             );
//         }
//         
//         [Theory]
//         [CustomizedAutoData]
//         public async Task Scheduled_domain_event_is_published_in_defined_time(EntityId orderId, ShoppingCart shoppingCart)
//         {
//             // Arrange
//             IClockService clockService = new ClockService();
//
//             await OrderHelper.PlaceOrderAsync(shoppingCart, orderId, clockService.Now);
//
//             IScheduledMessage scheduledMessage = new ScheduledMessage
//             (
//                 new PaymentGracePeriodExpiredDomainEvent(orderId),
//                 clockService.Now
//             );
//
//             // Act
//             await IntegrationTestsFixture.ExecuteServiceAsync<ISchedulerService>(sut =>
//                 sut.ScheduleMessageAsync(scheduledMessage));
//
//             // Assert
//             await IntegrationTestsFixture.AssertEventuallyAsync
//             (
//                 clockService,
//                 new GetScheduledMessageStatusProbe(),
//                 3000
//             );
//         }
//         
//         private class GetScheduledMessageStatusProbe : IProbe
//         {
//             private MessageLog _messageLog;
//
//             public bool IsSatisfied() 
//                 => _messageLog is { Status: MessageStatus.Finished };
//
//             public async Task SampleAsync()
//                 => _messageLog = await IntegrationTestsFixture.ExecuteServiceAsync<SchedulerContext, MessageLog>
//                     (
//                         dbContext => dbContext.MessageLogs
//                         .OrderByDescending(ml => ml.DateCreated)
//                         .FirstOrDefaultAsync()
//                     );
//
//             public string DescribeFailureTo() 
//                 => "Finished message log cannot be found!";
//         }
//     }
// }