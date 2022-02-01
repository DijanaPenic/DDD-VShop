using Xunit;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

using VShop.Modules.Billing.Integration.Events;
using VShop.Modules.ProcessManager.Infrastructure.Messages.Events;
using VShop.Modules.ProcessManager.Infrastructure.Messages.Reminders;
using VShop.Modules.ProcessManager.Infrastructure.ProcessManagers.Ordering;
using VShop.Modules.ProcessManager.Tests.Helpers;
using VShop.Modules.ProcessManager.Tests.IntegrationTests.Infrastructure;
using VShop.SharedKernel.Domain.ValueObjects;
using VShop.SharedKernel.Infrastructure.Extensions;
using VShop.SharedKernel.Infrastructure.Services;
using VShop.SharedKernel.Infrastructure.Services.Contracts;
using VShop.SharedKernel.Scheduler.DAL;
using VShop.SharedKernel.Scheduler.DAL.Entities;
using VShop.SharedKernel.Tests.Customizations;

namespace VShop.Modules.ProcessManager.Tests.IntegrationTests
{
    [Collection("Non-Parallel Tests Collection")]
    public class OrderingProcessManagerIntegrationTests : TestBase
    {
        [Theory, CustomAutoData]
        internal async Task Process_manager_schedules_a_reminder_message
        (
            EntityId shoppingCartId,
            EntityId orderId,
            Price deliveryCost,
            Discount customerDiscount,
            EntityId customerId,
            FullName fullName,
            Address deliveryAddress,
            PhoneNumber phoneNumber,
            EmailAddress emailAddress
        )
        {
            // Arrange
            IClockService clockService = new ClockService();
            
            OrderingProcessManager processManager = new();
            processManager.Transition
            (
                new ShoppingCartCheckoutRequestedDomainEvent
                (
                    shoppingCartId,
                    orderId,
                    clockService.Now
                ),
                clockService.Now
            );
            processManager.Transition
            (
                new OrderPlacedDomainEvent
                (
                    orderId,
                    deliveryCost,
                    customerDiscount,
                    customerId,
                    fullName.FirstName,
                    fullName.MiddleName,
                    fullName.LastName,
                    emailAddress,
                    phoneNumber,
                    deliveryAddress.City,
                    deliveryAddress.CountryCode,
                    deliveryAddress.PostalCode,
                    deliveryAddress.StateProvince,
                    deliveryAddress.StreetAddress
                ),
                clockService.Now
            );

            await ProcessManagerHelper.SaveProcessManagerAsync(processManager);

            PaymentSucceededIntegrationEvent paymentSucceededIntegrationEvent = new(orderId);
        
            // Act
            await ProcessManagerModule.PublishAsync(paymentSucceededIntegrationEvent);
            
            // Assert - a reminder message should have been scheduled.
            await ProcessManagerModule.ExecuteServiceAsync<SchedulerDbContext>(async dbContext =>
            {
                 string typeName = ScheduledMessageLog.ToName<PaymentGracePeriodExpiredDomainEvent>
                     (ProcessManagerModule.MessageRegistry);
                 
                 ScheduledMessageLog messageLog = await dbContext.MessageLogs
                     .OrderByDescending(ml => ml.DateCreated)
                     .FirstOrDefaultAsync(ml => ml.TypeName == typeName);
                 
                 messageLog.Should().NotBeNull();
                 messageLog!.GetMessage(ProcessManagerModule.MessageRegistry)
                     .Message.Should().BeOfType<PaymentGracePeriodExpiredDomainEvent>()
                     .Which.OrderId.Should().Be(orderId.Value.ToUuid());
            });
        }
    }
}