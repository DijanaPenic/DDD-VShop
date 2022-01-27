using Xunit;
using Microsoft.EntityFrameworkCore;

using VShop.Modules.Sales.Domain.Events.Reminders;
using VShop.Modules.Sales.Domain.Models.ShoppingCart;
using VShop.Modules.Sales.Infrastructure.Commands;
using VShop.Modules.Sales.Tests.Customizations;
using VShop.Modules.Sales.Tests.IntegrationTests.Helpers;
using VShop.Modules.Sales.Tests.IntegrationTests.Infrastructure;
using VShop.SharedKernel.Domain.ValueObjects;
using VShop.SharedKernel.Infrastructure.Messaging;
using VShop.SharedKernel.Infrastructure.Messaging.Contracts;
using VShop.SharedKernel.Infrastructure.Services;
using VShop.SharedKernel.Infrastructure.Services.Contracts;
using VShop.SharedKernel.Scheduler.DAL;
using VShop.SharedKernel.Scheduler.DAL.Entities;
using VShop.SharedKernel.Scheduler.Services.Contracts;
using VShop.SharedKernel.Tests.IntegrationTests.Probing;

namespace VShop.Modules.Sales.Tests.IntegrationTests
{
    [Collection("Non-Parallel Tests Collection")]
    public class SchedulerIntegrationTests : ResetDatabaseLifetime, IClassFixture<SchedulerFixture>
    {
        [Theory]
        [CustomizedAutoData]
        internal async Task Scheduled_command_is_published_in_defined_time
        (
            ShoppingCart shoppingCart,
            MessageMetadata metadata
        )
        {
            // Arrange
            IClockService clockService = new ClockService();

            await ShoppingCartHelper.SaveAndPublishAsync(shoppingCart);
        
            IScheduledMessage scheduledMessage = new ScheduledMessage
            (
                new DeleteShoppingCartCommand(shoppingCart.Id, metadata),
                clockService.Now
            );
        
            // Act
            await IntegrationTestsFixture.ExecuteServiceAsync<ISchedulerService>(sut =>
                sut.ScheduleMessageAsync(scheduledMessage));
        
            // Assert
            await IntegrationTestsFixture.AssertEventuallyAsync
            (
                clockService,
                new GetScheduledMessageStatusProbe(),
                3000
            );
        }
        
        [Theory]
        [CustomizedAutoData]
        internal async Task Scheduled_domain_event_is_published_in_defined_time
        (
            EntityId orderId, 
            ShoppingCart shoppingCart,
            MessageMetadata metadata
        )
        {
            // Arrange
            IClockService clockService = new ClockService();

            await OrderHelper.PlaceOrderAsync(shoppingCart, orderId, clockService.Now);

            IScheduledMessage scheduledMessage = new ScheduledMessage
            (
                new PaymentGracePeriodExpiredDomainEvent(orderId) { Metadata = metadata },
                clockService.Now
            );

            // Act
            await IntegrationTestsFixture.ExecuteServiceAsync<ISchedulerService>(sut =>
                sut.ScheduleMessageAsync(scheduledMessage));

            // Assert
            await IntegrationTestsFixture.AssertEventuallyAsync
            (
                clockService,
                new GetScheduledMessageStatusProbe(),
                3000
            );
        }
        
        private class GetScheduledMessageStatusProbe : IProbe
        {
            private ScheduledMessageLog _messageLog;

            public bool IsSatisfied() => _messageLog is { Status: MessageStatus.Finished };

            public async Task SampleAsync()
                => _messageLog = await IntegrationTestsFixture.ExecuteServiceAsync<SchedulerDbContext, ScheduledMessageLog>
                    (
                        dbContext => dbContext.MessageLogs
                        .OrderByDescending(ml => ml.DateCreated)
                        .FirstOrDefaultAsync()
                    );

            public string DescribeFailureTo() => "Finished message log cannot be found!";
        }
    }
}