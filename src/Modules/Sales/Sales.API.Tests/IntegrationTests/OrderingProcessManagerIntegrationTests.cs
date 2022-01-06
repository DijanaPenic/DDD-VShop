using Xunit;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

using VShop.SharedKernel.Messaging;
using VShop.SharedKernel.Domain.ValueObjects;
using VShop.SharedKernel.Infrastructure.Services;
using VShop.SharedKernel.Infrastructure.Services.Contracts;
using VShop.SharedKernel.Scheduler.Infrastructure;
using VShop.SharedKernel.Scheduler.Infrastructure.Entities;
using VShop.Modules.Sales.Domain.Enums;
using VShop.Modules.Sales.Domain.Events;
using VShop.Modules.Sales.Domain.Models.Ordering;
using VShop.Modules.Sales.Domain.Models.ShoppingCart;
using VShop.Modules.Sales.Tests.Customizations;
using VShop.Modules.Sales.API.Application.ProcessManagers;
using VShop.Modules.Sales.API.Tests.IntegrationTests.Helpers;
using VShop.Modules.Sales.API.Tests.IntegrationTests.Infrastructure;
using VShop.Modules.Billing.Integration.Events;

namespace VShop.Modules.Sales.API.Tests.IntegrationTests
{
    public class OrderingProcessManagerIntegrationTests
    {
        [Theory]
        [CustomizedAutoData]
        public async Task Process_manager_handles_commands_and_scheduled_messages
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
            await IntegrationTestsFixture.ExecuteServiceAsync<OrderingProcessManagerHandler>(sut =>
                 sut.Handle(paymentSucceededIntegrationEvent, CancellationToken.None));
            
            // Assert
            
            // A command should have been executed.
            Order orderFromDb = await OrderHelper.GetOrderAsync(orderId);
            orderFromDb.Should().NotBeNull();
            orderFromDb.Status.Should().Be(OrderStatus.Paid);
            
            // A reminder message should have been scheduled.
            await IntegrationTestsFixture.ExecuteServiceAsync<SchedulerContext>(async dbContext =>
            {
                 string typeName = ScheduledMessage.ToName<OrderStockProcessingGracePeriodExpiredDomainEvent>();
                 
                 MessageLog messageLog = await dbContext.MessageLogs
                     .OrderByDescending(ml => ml.DateCreated)
                     .FirstOrDefaultAsync(ml => ml.TypeName == typeName);
                 messageLog.Should().NotBeNull();

                 OrderStockProcessingGracePeriodExpiredDomainEvent stockReminder = messageLog!
                     .GetMessage<OrderStockProcessingGracePeriodExpiredDomainEvent>();
                 stockReminder.Should().NotBeNull();
                 stockReminder!.OrderId.Should().Be(orderId);
            });
        }
    }
}