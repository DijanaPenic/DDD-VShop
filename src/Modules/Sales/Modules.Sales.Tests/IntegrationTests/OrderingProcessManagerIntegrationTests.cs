using Xunit;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

using VShop.Modules.Billing.Integration.Events;
using VShop.Modules.Sales.Domain.Enums;
using VShop.Modules.Sales.Domain.Events.Reminders;
using VShop.Modules.Sales.Domain.Models.Ordering;
using VShop.Modules.Sales.Domain.Models.ShoppingCart;
using VShop.Modules.Sales.Tests.Customizations;
using VShop.Modules.Sales.Tests.IntegrationTests.Helpers;
using VShop.Modules.Sales.Tests.IntegrationTests.Infrastructure;
using VShop.SharedKernel.Domain.ValueObjects;
using VShop.SharedKernel.Infrastructure.Extensions;
using VShop.SharedKernel.Infrastructure.Messaging;
using VShop.SharedKernel.Infrastructure.Services;
using VShop.SharedKernel.Infrastructure.Services.Contracts;
using VShop.SharedKernel.Scheduler.DAL;
using VShop.SharedKernel.Scheduler.DAL.Entities;

namespace VShop.Modules.Sales.Tests.IntegrationTests
{
    [Collection("Non-Parallel Tests Collection")]
    public class OrderingProcessManagerIntegrationTests
    {
        [Theory]
        [CustomizedAutoData]
        internal async Task Process_manager_handles_commands_and_scheduled_messages
        (
            ShoppingCart shoppingCart,
            EntityId orderId
        )
        {
            // Arrange
            IClockService clockService = new ClockService();
            await OrderHelper.PlaceOrderAsync(shoppingCart, orderId, clockService.Now);

            PaymentSucceededIntegrationEvent paymentSucceededIntegrationEvent = new(orderId);
        
            // Act
            await IntegrationTestsFixture.PublishAsync(paymentSucceededIntegrationEvent);
            
            // Assert
            
            // A command should have been executed.
            Order orderFromDb = await OrderHelper.GetOrderAsync(orderId);
            orderFromDb.Should().NotBeNull();
            orderFromDb.Status.Should().Be(OrderStatus.Paid);
            
            // A reminder message should have been scheduled.
            await IntegrationTestsFixture.ExecuteServiceAsync<SchedulerDbContext>(async dbContext =>
            {
                 string typeName = ScheduledMessageLog.ToName<OrderStockProcessingGracePeriodExpiredDomainEvent>
                     (IntegrationTestsFixture.MessageRegistry);
                 
                 ScheduledMessageLog messageLog = await dbContext.MessageLogs
                     .OrderByDescending(ml => ml.DateCreated)
                     .FirstOrDefaultAsync(ml => ml.TypeName == typeName);
                 
                 messageLog.Should().NotBeNull();
                 messageLog!.GetMessage(IntegrationTestsFixture.MessageRegistry).Should()
                     .NotBeNull().And
                     .BeOfType<OrderStockProcessingGracePeriodExpiredDomainEvent>()
                     .Which.OrderId.Should().Be(orderId.Value.ToUuid());
            });
        }
    }
}