// using Xunit;
// using AutoFixture;
// using FluentAssertions;
// using System.Threading.Tasks;
// using Microsoft.EntityFrameworkCore;
//
// using VShop.SharedKernel.Tests;
// using VShop.SharedKernel.Messaging;
// using VShop.SharedKernel.Domain.ValueObjects;
// using VShop.SharedKernel.Scheduler.Infrastructure;
// using VShop.SharedKernel.Scheduler.Infrastructure.Entities;
// using VShop.SharedKernel.Scheduler.Services.Contracts;
// using VShop.SharedKernel.Infrastructure.Services.Contracts;
// using VShop.SharedKernel.EventSourcing.Repositories.Contracts;
// using VShop.Modules.Sales.API.Application.Commands;
// using VShop.Modules.Sales.Domain.Models.ShoppingCart;
//
// namespace VShop.Modules.Sales.API.Tests.IntegrationTests
// {
//     // TODO - work in progress
//     [Collection("Integration Tests Collection")]
//     public class SchedulerIntegrationTests : IntegrationTestsBase
//     {
//         private readonly Fixture _autoFixture;
//
//         public SchedulerIntegrationTests(AppFixture appFixture) => _autoFixture = appFixture.AutoFixture;
//
//         [Fact]
//         public async Task Scheduled_command_is_published_in_defined_time()
//         {
//             // Arrange
//             SchedulerContext schedulerContext = GetService<SchedulerContext>();
//             IAggregateRepository<ShoppingCart, EntityId> shoppingCartRepository = 
//                 GetService<IAggregateRepository<ShoppingCart, EntityId>>();
//             IClockService clockService = GetService<IClockService>();
//             ISchedulerService sut = GetService<ISchedulerService>();
//             
//             // TODO - maybe move to attribute
//             ShoppingCart shoppingCart = ShoppingCartFixture.GetShoppingCartForCheckoutFixture(_autoFixture);
//             shoppingCart.RequestCheckout(clockService, _autoFixture.Create<EntityId>());
//             
//             await shoppingCartRepository.SaveAsync(shoppingCart);
//
//             IScheduledMessage scheduledMessage = new ScheduledMessage
//             (
//                 new DeleteShoppingCartCommand(shoppingCart.Id),
//                 clockService.Now
//             );
//
//             // Act
//             await sut.ScheduleMessageAsync(scheduledMessage);
//
//             // Assert
//             MessageLog messageLog = await schedulerContext.MessageLogs.SingleOrDefaultAsync();
//             messageLog.Should().NotBeNull();
//             messageLog!.Status.Should().Be(MessageStatus.Finished);
//         }
//     }
// }