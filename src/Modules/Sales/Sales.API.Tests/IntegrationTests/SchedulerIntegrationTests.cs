using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

using VShop.SharedKernel.Messaging;
using VShop.SharedKernel.Scheduler.Infrastructure;
using VShop.SharedKernel.Scheduler.Infrastructure.Entities;
using VShop.SharedKernel.Scheduler.Services.Contracts;
using VShop.SharedKernel.Infrastructure.Services;
using VShop.SharedKernel.Infrastructure.Services.Contracts;
using VShop.Modules.Sales.Domain.Events;
using VShop.Modules.Sales.Domain.Models.ShoppingCart;
using VShop.Modules.Sales.Tests.Customizations;
using VShop.Modules.Sales.API.Application.Commands;
using VShop.Modules.Sales.API.Tests.IntegrationTests.Helpers;
using VShop.Modules.Sales.API.Tests.IntegrationTests.Infrastructure;

namespace VShop.Modules.Sales.API.Tests.IntegrationTests
{
    // TODO - work in progress
    [Collection("Integration Tests Collection")]
    public class SchedulerIntegrationTests : ResetDatabaseLifetime
    {
        [Theory]
        [CustomizedAutoData]
        public async Task Scheduled_command_is_published_in_defined_time(Guid orderId, ShoppingCart shoppingCart)
        {
            // Arrange
            IClockService clockService = new ClockService();
            
            await OrderHelper.PlaceOrderAsync(clockService, shoppingCart, orderId);
        
            IScheduledMessage scheduledMessage = new ScheduledMessage
            (
                new DeleteShoppingCartCommand(shoppingCart.Id),
                clockService.Now
            );
        
            // Act
            await IntegrationTestsFixture.ExecuteServiceAsync<ISchedulerService>(sut =>
                sut.ScheduleMessageAsync(scheduledMessage));
        
            // Assert
            Thread.Sleep(3000); // TODO - timeout
            
            await IntegrationTestsFixture.ExecuteServiceAsync<SchedulerContext>(async dbContext =>
            {
                MessageLog messageLog = await dbContext.MessageLogs.SingleOrDefaultAsync();
                messageLog.Should().NotBeNull();
                messageLog!.Status.Should().Be(MessageStatus.Finished);
            });
        }
        
        [Theory]
        [CustomizedAutoData]
        public async Task Scheduled_domain_event_is_published_in_defined_time(Guid orderId, ShoppingCart shoppingCart)
        {
            // Arrange
            IClockService clockService = new ClockService();

            await OrderHelper.PlaceOrderAsync(clockService, shoppingCart, orderId);

            IScheduledMessage scheduledMessage = new ScheduledMessage
            (
                new PaymentGracePeriodExpiredDomainEvent(orderId),
                clockService.Now
            );

            // Act
            await IntegrationTestsFixture.ExecuteServiceAsync<ISchedulerService>(sut =>
                sut.ScheduleMessageAsync(scheduledMessage));

            // Assert
            Thread.Sleep(3000);

            await IntegrationTestsFixture.ExecuteServiceAsync<SchedulerContext>(async dbContext =>
            {
                MessageLog messageLog = await dbContext.MessageLogs.SingleOrDefaultAsync();
                messageLog.Should().NotBeNull();
                messageLog!.Status.Should().Be(MessageStatus.Finished); 
            });
        }
    }
}