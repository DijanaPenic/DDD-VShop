using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using AutoFixture;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

using VShop.SharedKernel.Tests;
using VShop.SharedKernel.Messaging;
using VShop.SharedKernel.Scheduler.Infrastructure;
using VShop.SharedKernel.Scheduler.Infrastructure.Entities;
using VShop.SharedKernel.Scheduler.Services.Contracts;
using VShop.SharedKernel.Infrastructure.Services;
using VShop.SharedKernel.Infrastructure.Services.Contracts;
using VShop.Modules.Sales.Domain.Events;
using VShop.Modules.Sales.Domain.Models.ShoppingCart;
using VShop.Modules.Sales.API.Application.Commands;
using VShop.Modules.Sales.API.Tests.IntegrationTests.Helpers;
using VShop.Modules.Sales.API.Tests.IntegrationTests.Infrastructure;

namespace VShop.Modules.Sales.API.Tests.IntegrationTests
{
    // TODO - work in progress
    [Collection("Integration Tests Collection")]
    public class SchedulerIntegrationTests : ResetDatabaseLifetime
    {
        private readonly Fixture _autoFixture;
        private readonly ShoppingCartHelper _shoppingCartHelper;

        public SchedulerIntegrationTests(AppFixture appFixture)
        {
            _autoFixture = appFixture.AutoFixture;
            _shoppingCartHelper = new ShoppingCartHelper(_autoFixture);
        }

        [Fact]
        public async Task Scheduled_command_is_published_in_defined_time()
        {
            // Arrange
            IClockService clockService = new ClockService();
            Guid orderId = _autoFixture.Create<Guid>();
            
            // TODO - maybe move to attribute
            ShoppingCart shoppingCart = await _shoppingCartHelper.CheckoutShoppingCartAsync(clockService, orderId);
        
            IScheduledMessage scheduledMessage = new ScheduledMessage
            (
                new DeleteShoppingCartCommand(shoppingCart.Id), // TODO - for some reason command mapping is not working
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
        
        [Fact]
        public async Task Scheduled_domain_event_is_published_in_defined_time()
        {
            // Arrange
            IClockService clockService = new ClockService();
            Guid orderId = _autoFixture.Create<Guid>();
            
            await _shoppingCartHelper.CheckoutShoppingCartAsync(clockService, orderId);

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