using Xunit;
using Microsoft.EntityFrameworkCore;

using VShop.Tests.IntegrationTests.Helpers;
using VShop.SharedKernel.Domain.ValueObjects;
using VShop.SharedKernel.Infrastructure.Contexts.Contracts;
using VShop.SharedKernel.Infrastructure.Messaging;
using VShop.SharedKernel.Infrastructure.Messaging.Contracts;
using VShop.SharedKernel.Infrastructure.Services;
using VShop.SharedKernel.Infrastructure.Services.Contracts;
using VShop.SharedKernel.Scheduler.DAL;
using VShop.SharedKernel.Scheduler.DAL.Entities;
using VShop.SharedKernel.Scheduler.Services.Contracts;
using VShop.SharedKernel.Tests.Customizations;
using VShop.SharedKernel.Tests.IntegrationTests.Probing.Contracts;
using VShop.Tests.IntegrationTests.Infrastructure;
using VShop.Modules.ProcessManager.Infrastructure.Messages.Commands;
using VShop.Modules.Sales.Domain.Models.ShoppingCart;
using VShop.Modules.ProcessManager.Infrastructure.Messages.Reminders;

namespace VShop.Tests.IntegrationTests.Scheduler
{
    [Collection("Non-Parallel Tests Collection")]
    public class SchedulerIntegrationTests : TestBase
    {
        [Theory, CustomAutoData]
        internal async Task Scheduled_command_is_published_in_defined_time
        (
            ShoppingCart shoppingCart,
            EntityId orderId,
            IContext context
        )
        {
            // Arrange
            IClockService clockService = new ClockService();

            await ShoppingCartHelper.SaveAsync(shoppingCart);

            IScheduledMessage scheduledMessage = new ScheduledMessage
            (
                new PlaceOrderCommand(orderId, shoppingCart.Id),
                clockService.Now
            );

            // Act
            await ProcessManagerModule.ExecuteServiceAsync<ISchedulerService>(sut =>
                sut.ScheduleMessageAsync
                    (new MessageEnvelope<IScheduledMessage>(scheduledMessage, new MessageContext(context))));

            // Assert
            await ProcessManagerModule.AssertEventuallyAsync
            (
                clockService,
                new GetScheduledMessageStatusProbe(),
                3000
            );
        }
        
        [Theory, CustomAutoData]
        internal async Task Scheduled_domain_event_is_published_in_defined_time
        (
            EntityId orderId, 
            ShoppingCart shoppingCart,
            IContext context
        )
        {
            // Arrange
            IClockService clockService = new ClockService();

            await OrderHelper.PlaceOrderAsync(shoppingCart, orderId, clockService.Now);

            IScheduledMessage scheduledMessage = new ScheduledMessage
            (
                new PaymentGracePeriodExpiredDomainEvent(orderId),
                clockService.Now
            );

            // Act
            await ProcessManagerModule.ExecuteServiceAsync<ISchedulerService>(sut =>
                sut.ScheduleMessageAsync(new MessageEnvelope<IScheduledMessage>(scheduledMessage, new MessageContext(context))));

            // Assert
            await ProcessManagerModule.AssertEventuallyAsync
            (
                clockService,
                new GetScheduledMessageStatusProbe(),
                3000
            );
        }
        
        private class GetScheduledMessageStatusProbe : IProbe
        {
            private ScheduledMessageLog _messageLog;

            public bool IsSatisfied() => _messageLog is { Status: ScheduledMessageStatus.Finished };

            public async Task SampleAsync()
                => _messageLog = await ProcessManagerModule.ExecuteServiceAsync<SchedulerDbContext, ScheduledMessageLog>
                    (
                        dbContext => dbContext.MessageLogs
                        .OrderByDescending(ml => ml.DateCreated)
                        .FirstOrDefaultAsync()
                    );

            public string DescribeFailureTo() => "Finished message log cannot be found!";
        }
    }
}