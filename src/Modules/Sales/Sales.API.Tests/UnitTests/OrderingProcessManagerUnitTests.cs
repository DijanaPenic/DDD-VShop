using Xunit;
using System.Linq;
using System.Collections.Generic;
using FluentAssertions;

using VShop.SharedKernel.Messaging;
using VShop.SharedKernel.Domain.ValueObjects;
using VShop.SharedKernel.Infrastructure.Types;
using VShop.SharedKernel.Infrastructure.Services;
using VShop.SharedKernel.Infrastructure.Services.Contracts;
using VShop.Modules.Sales.Domain.Events;
using VShop.Modules.Sales.Domain.Events.Reminders;
using VShop.Modules.Sales.Tests.Customizations;
using VShop.Modules.Sales.API.Application.Commands;
using VShop.Modules.Sales.API.Application.ProcessManagers;
using VShop.Modules.Billing.Integration.Events;
using VShop.Modules.Catalog.Integration.Events;

namespace VShop.Modules.Sales.API.Tests.UnitTests
{
    public class OrderingProcessManagerUnitTests
    {
        [Theory]
        [CustomizedAutoData]
        public void Shopping_cart_checkout_places_a_new_order
        (
            EntityId shoppingCartId,
            EntityId orderId
        )
        {
            // Arrange
            IClockService clockService = new ClockService();
            
            OrderingProcessManager processManager = new();
            ShoppingCartCheckoutRequestedDomainEvent shoppingCartCheckoutRequestedDomainEvent = new
            (
                shoppingCartId,
                orderId,
                clockService.Now
            );

            // Act
            processManager.Transition(shoppingCartCheckoutRequestedDomainEvent, clockService.Now);
            
            // Assert
            PlaceOrderCommand placeOrderCommand = processManager.Outbox.Messages
                .OfType<PlaceOrderCommand>()
                .SingleOrDefault();
            placeOrderCommand.Should().NotBeNull();

            processManager.Status.Should().Be(OrderingProcessManagerStatus.CheckoutRequested);
        }
        
        [Theory]
        [CustomizedAutoData]
        public void Creating_a_new_order_deletes_the_shopping_cart_and_schedules_a_payment_reminder
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

            OrderPlacedDomainEvent orderPlacedDomainEvent = new
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
            );

            // Act
            processManager.Transition(orderPlacedDomainEvent, clockService.Now);
            
            // Assert
            DeleteShoppingCartCommand deleteShoppingCartCommand = processManager.Outbox.Messages
                .OfType<DeleteShoppingCartCommand>()
                .SingleOrDefault();
            deleteShoppingCartCommand.Should().NotBeNull();

            IScheduledMessage paymentReminder = processManager.Outbox.Messages.OfType<IScheduledMessage>()
                .SingleOrDefault(sm => sm.TypeName == ScheduledMessage.ToName<PaymentGracePeriodExpiredDomainEvent>());
            paymentReminder.Should().NotBeNull();

            processManager.Status.Should().Be(OrderingProcessManagerStatus.OrderPlaced);
        }
        
        [Theory]
        [CustomizedAutoData]
        public void Failed_order_payment_changes_the_process_manager_status
        (
            EntityId shoppingCartId,
            EntityId orderId,
            Price deliveryCost,
            Discount customerDiscount,
            EntityId customerId,
            FullName fullName,
            Address deliveryAddress,
            PhoneNumber phoneNumber,
            EmailAddress emailAddress,
            MessageMetadata paymentMetadata
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

            int expectedMessagesCount = processManager.Outbox.Messages.Count;
            PaymentFailedIntegrationEvent paymentFailedIntegrationEvent = new(orderId, paymentMetadata);

            // Act
            processManager.Transition(paymentFailedIntegrationEvent, clockService.Now);
            
            // Assert
            processManager.Outbox.Messages.Count.Should().Be(expectedMessagesCount);
            processManager.Status.Should().Be(OrderingProcessManagerStatus.OrderPaymentFailed);
        }
        
        [Theory]
        [CustomizedAutoData]
        public void Payment_reminder_check_sends_an_alert_when_payment_response_is_not_received
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

            PaymentGracePeriodExpiredDomainEvent paymentGracePeriodExpiredDomainEvent = new(orderId);

            // Act
            processManager.Transition(paymentGracePeriodExpiredDomainEvent, clockService.Now);
            
            // Assert
            // TODO - missing implementation
            
            processManager.Status.Should().Be(OrderingProcessManagerStatus.OrderPlaced);
        }
        
        [Theory]
        [CustomizedAutoData]
        public void Payment_reminder_check_doesnt_react_when_order_is_paid
        (
            EntityId shoppingCartId,
            EntityId orderId,
            Price deliveryCost,
            Discount customerDiscount,
            EntityId customerId,
            FullName fullName,
            Address deliveryAddress,
            PhoneNumber phoneNumber,
            EmailAddress emailAddress,
            MessageMetadata paymentMetadata
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
            processManager.Transition
            (
                new PaymentSucceededIntegrationEvent(orderId, paymentMetadata),
                clockService.Now
            );

            int expectedMessagesCount = processManager.Outbox.Messages.Count;
            PaymentGracePeriodExpiredDomainEvent paymentGracePeriodExpiredDomainEvent = new(orderId);

            // Act
            processManager.Transition(paymentGracePeriodExpiredDomainEvent, clockService.Now);
            
            // Assert
            processManager.Outbox.Messages.Count.Should().Be(expectedMessagesCount);
            processManager.Status.Should().Be(OrderingProcessManagerStatus.OrderPaymentSucceeded);
        }
        
        [Theory]
        [CustomizedAutoData]
        public void Payment_reminder_check_cancels_the_order_when_order_is_not_paid
        (
            EntityId shoppingCartId,
            EntityId orderId,
            Price deliveryCost,
            Discount customerDiscount,
            EntityId customerId,
            FullName fullName,
            Address deliveryAddress,
            PhoneNumber phoneNumber,
            EmailAddress emailAddress,
            MessageMetadata paymentMetadata
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
            processManager.Transition
            (
                new PaymentFailedIntegrationEvent(orderId, paymentMetadata),
                clockService.Now
            );

            PaymentGracePeriodExpiredDomainEvent paymentGracePeriodExpiredDomainEvent = new(orderId);

            // Act
            processManager.Transition(paymentGracePeriodExpiredDomainEvent, clockService.Now);
            
            // Assert
            CancelOrderCommand cancelOrderCommand = processManager.Outbox.Messages
                .OfType<CancelOrderCommand>()
                .SingleOrDefault();
            cancelOrderCommand.Should().NotBeNull();

            processManager.Status.Should().Be(OrderingProcessManagerStatus.OrderPaymentFailed);
        }
        
        [Theory]
        [CustomizedAutoData]
        public void Cancelled_order_changes_the_process_manager_status
        (
            EntityId shoppingCartId,
            EntityId orderId,
            Price deliveryCost,
            Discount customerDiscount,
            EntityId customerId,
            FullName fullName,
            Address deliveryAddress,
            PhoneNumber phoneNumber,
            EmailAddress emailAddress,
            MessageMetadata paymentMetadata
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
            processManager.Transition
            (
                new PaymentFailedIntegrationEvent(orderId, paymentMetadata),
                clockService.Now
            );
            processManager.Transition
            (
                new PaymentGracePeriodExpiredDomainEvent(orderId),
                clockService.Now
            );

            int expectedMessagesCount = processManager.Outbox.Messages.Count;
            OrderStatusSetToCancelledDomainEvent orderStatusSetToCancelled = new(orderId);

            // Act
            processManager.Transition(orderStatusSetToCancelled, clockService.Now);
            
            // Assert
            processManager.Outbox.Messages.Count.Should().Be(expectedMessagesCount);
            processManager.Status.Should().Be(OrderingProcessManagerStatus.OrderCancelled);
        }
        
        [Theory]
        [CustomizedAutoData]
        public void Succeeded_order_payment_changes_the_order_status_to_paid
        (
            EntityId shoppingCartId,
            EntityId orderId,
            Price deliveryCost,
            Discount customerDiscount,
            EntityId customerId,
            FullName fullName,
            Address deliveryAddress,
            PhoneNumber phoneNumber,
            EmailAddress emailAddress,
            MessageMetadata paymentMetadata
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

            PaymentSucceededIntegrationEvent paymentSucceededIntegrationEvent = new(orderId, paymentMetadata);

            // Act
            processManager.Transition(paymentSucceededIntegrationEvent, clockService.Now);
            
            // Assert
            SetPaidOrderStatusCommand setPaidOrderStatusCommand = processManager.Outbox.Messages
                .OfType<SetPaidOrderStatusCommand>()
                .SingleOrDefault();
            setPaidOrderStatusCommand.Should().NotBeNull();
            
            processManager.Status.Should().Be(OrderingProcessManagerStatus.OrderPaymentSucceeded);
        }
        
        [Theory]
        [CustomizedAutoData]
        public void Changing_the_order_status_to_Paid_schedules_a_stock_reminder
        (
            EntityId shoppingCartId,
            EntityId orderId,
            Price deliveryCost,
            Discount customerDiscount,
            EntityId customerId,
            FullName fullName,
            Address deliveryAddress,
            PhoneNumber phoneNumber,
            EmailAddress emailAddress,
            MessageMetadata paymentMetadata
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
            processManager.Transition
            (
                new PaymentSucceededIntegrationEvent(orderId, paymentMetadata),
                clockService.Now
            );

            OrderStatusSetToPaidDomainEvent orderStatusSetToPaidDomainEvent = new(orderId);

            // Act
            processManager.Transition(orderStatusSetToPaidDomainEvent, clockService.Now);
            
            // Assert
            IScheduledMessage stockReminder = processManager.Outbox.Messages.OfType<IScheduledMessage>()
                .SingleOrDefault(sm => sm.TypeName == ScheduledMessage.ToName<OrderStockProcessingGracePeriodExpiredDomainEvent>());
            stockReminder.Should().NotBeNull();

            processManager.Status.Should().Be(OrderingProcessManagerStatus.OrderPaymentSucceeded);
        }
        
        [Theory]
        [CustomizedAutoData]
        public void Stock_reminder_check_sends_an_alert_when_stock_review_response_is_not_received
        (
            EntityId shoppingCartId,
            EntityId orderId,
            Price deliveryCost,
            Discount customerDiscount,
            EntityId customerId,
            FullName fullName,
            Address deliveryAddress,
            PhoneNumber phoneNumber,
            EmailAddress emailAddress,
            MessageMetadata paymentMetadata
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
            processManager.Transition
            (
                new PaymentSucceededIntegrationEvent(orderId, paymentMetadata),
                clockService.Now
            );
            processManager.Transition
            (
                new OrderStatusSetToPaidDomainEvent(orderId),
                clockService.Now
            );
            
            OrderStockProcessingGracePeriodExpiredDomainEvent orderStockProcessingGracePeriodExpiredDomainEvent = new(orderId);

            // Act
            processManager.Transition(orderStockProcessingGracePeriodExpiredDomainEvent, clockService.Now);
            
            // Assert
            // TODO - missing implementation

            processManager.Status.Should().Be(OrderingProcessManagerStatus.OrderPaymentSucceeded);
        }
        
        [Theory]
        [CustomizedAutoData]
        public void Stock_reminder_check_doesnt_react_when_order_stock_is_confirmed
        (
            EntityId shoppingCartId,
            EntityId orderId,
            Price deliveryCost,
            Discount customerDiscount,
            EntityId customerId,
            FullName fullName,
            Address deliveryAddress,
            PhoneNumber phoneNumber,
            EmailAddress emailAddress,
            IList<OrderStockProcessedIntegrationEvent.Types.OrderLine> orderLines,
            MessageMetadata paymentMetadata,
            MessageMetadata stockMetadata
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
            processManager.Transition
            (
                new PaymentSucceededIntegrationEvent(orderId, paymentMetadata),
                clockService.Now
            );
            processManager.Transition
            (
                new OrderStatusSetToPaidDomainEvent(orderId),
                clockService.Now
            );
            processManager.Transition
            (
                new OrderStockProcessedIntegrationEvent(orderId, orderLines, stockMetadata),
                clockService.Now
            );
            
            int expectedMessagesCount = processManager.Outbox.Messages.Count;
            OrderStockProcessingGracePeriodExpiredDomainEvent orderStockProcessingGracePeriodExpiredDomainEvent = new(orderId);

            // Act
            processManager.Transition(orderStockProcessingGracePeriodExpiredDomainEvent, clockService.Now);
            
            // Assert
            processManager.Outbox.Messages.Count.Should().Be(expectedMessagesCount);
            processManager.Status.Should().Be(OrderingProcessManagerStatus.OrderStockConfirmed);
        }
        
        [Theory]
        [CustomizedAutoData]
        public void Processed_stock_order_report_finalizes_the_order
        (
            EntityId shoppingCartId,
            EntityId orderId,
            Price deliveryCost,
            Discount customerDiscount,
            EntityId customerId,
            FullName fullName,
            Address deliveryAddress,
            PhoneNumber phoneNumber,
            EmailAddress emailAddress,
            IList<OrderStockProcessedIntegrationEvent.Types.OrderLine> orderLines,
            MessageMetadata paymentMetadata,
            MessageMetadata stockMetadata
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
            processManager.Transition
            (
                new PaymentSucceededIntegrationEvent(orderId, paymentMetadata),
                clockService.Now
            );
            processManager.Transition
            (
                new OrderStatusSetToPaidDomainEvent(orderId),
                clockService.Now
            );

            OrderStockProcessedIntegrationEvent orderStockProcessedIntegrationEvent = new(orderId, orderLines, stockMetadata);

            // Act
            processManager.Transition(orderStockProcessedIntegrationEvent, clockService.Now);
            
            // Assert
            FinalizeOrderCommand finalizeOrderCommand = processManager.Outbox.Messages
                .OfType<FinalizeOrderCommand>()
                .SingleOrDefault();
            finalizeOrderCommand.Should().NotBeNull();

            processManager.Status.Should().Be(OrderingProcessManagerStatus.OrderStockConfirmed);
        }
        
        [Theory]
        [CustomizedAutoData]
        public void Changing_the_order_status_to_PendingShipping_schedules_a_shipping_reminder
        (
            EntityId shoppingCartId,
            EntityId orderId,
            Price deliveryCost,
            Discount customerDiscount,
            EntityId customerId,
            FullName fullName,
            Address deliveryAddress,
            PhoneNumber phoneNumber,
            EmailAddress emailAddress,
            MessageMetadata paymentMetadata,
            MessageMetadata stockMetadata
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
            processManager.Transition
            (
                new PaymentSucceededIntegrationEvent(orderId, paymentMetadata),
                clockService.Now
            );
            processManager.Transition
            (
                new OrderStatusSetToPaidDomainEvent(orderId),
                clockService.Now
            );
            processManager.Transition
            (
                new OrderStockProcessedIntegrationEvent
                (
                    orderId,
                    new List<OrderStockProcessedIntegrationEvent.Types.OrderLine>
                    {
                        new(SequentialGuid.Create(), 10, 0)
                    }, stockMetadata
                ),
                clockService.Now
            );

            OrderStatusSetToPendingShippingDomainEvent orderStatusSetToPendingShippingDomainEvent = new(orderId);

            // Act
            processManager.Transition(orderStatusSetToPendingShippingDomainEvent, clockService.Now);
            
            // Assert
            IScheduledMessage shippingReminder = processManager.Outbox.Messages.OfType<IScheduledMessage>()
                .SingleOrDefault(sm => sm.TypeName == ScheduledMessage.ToName<ShippingGracePeriodExpiredDomainEvent>());
            shippingReminder.Should().NotBeNull();

            processManager.Status.Should().Be(OrderingProcessManagerStatus.OrderPendingShipping);
        }
        
        [Theory]
        [CustomizedAutoData]
        public void Shipping_reminder_check_sends_an_alert_when_shipping_response_is_not_received
        (
            EntityId shoppingCartId,
            EntityId orderId,
            Price deliveryCost,
            Discount customerDiscount,
            EntityId customerId,
            FullName fullName,
            Address deliveryAddress,
            PhoneNumber phoneNumber,
            EmailAddress emailAddress,
            MessageMetadata paymentMetadata,
            MessageMetadata stockMetadata
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
            processManager.Transition
            (
                new PaymentSucceededIntegrationEvent(orderId, paymentMetadata),
                clockService.Now
            );
            processManager.Transition
            (
                new OrderStatusSetToPaidDomainEvent(orderId),
                clockService.Now
            );
            processManager.Transition
            (
                new OrderStockProcessedIntegrationEvent
                (
                    orderId,
                    new List<OrderStockProcessedIntegrationEvent.Types.OrderLine>
                    {
                        new(SequentialGuid.Create(), 10, 0)
                    }, stockMetadata
                ),
                clockService.Now
            );
            processManager.Transition
            (
                new OrderStatusSetToPendingShippingDomainEvent(orderId),
                clockService.Now
            );

            ShippingGracePeriodExpiredDomainEvent shippingGracePeriodExpiredDomainEvent = new(orderId);

            // Act
            processManager.Transition(shippingGracePeriodExpiredDomainEvent, clockService.Now);
            
            // Assert
            // TODO - missing implementation

            processManager.Status.Should().Be(OrderingProcessManagerStatus.OrderPendingShipping);
        }
        
        [Theory]
        [CustomizedAutoData]
        public void Shipping_reminder_check_doesnt_react_when_order_is_shipped
        (
            EntityId shoppingCartId,
            EntityId orderId,
            Price deliveryCost,
            Discount customerDiscount,
            EntityId customerId,
            FullName fullName,
            Address deliveryAddress,
            PhoneNumber phoneNumber,
            EmailAddress emailAddress,
            MessageMetadata paymentMetadata,
            MessageMetadata stockMetadata
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
            processManager.Transition
            (
                new PaymentSucceededIntegrationEvent(orderId, paymentMetadata),
                clockService.Now
            );
            processManager.Transition
            (
                new OrderStatusSetToPaidDomainEvent(orderId),
                clockService.Now
            );
            processManager.Transition
            (
                new OrderStockProcessedIntegrationEvent
                (
                    orderId,
                    new List<OrderStockProcessedIntegrationEvent.Types.OrderLine>
                    {
                        new(SequentialGuid.Create(), 10, 0)
                    }, stockMetadata
                ),
                clockService.Now
            );
            processManager.Transition
            (
                new OrderStatusSetToPendingShippingDomainEvent(orderId),
                clockService.Now
            );
            processManager.Transition
            (
                new OrderStatusSetToShippedDomainEvent(orderId),
                clockService.Now
            );

            int expectedMessagesCount = processManager.Outbox.Messages.Count;
            ShippingGracePeriodExpiredDomainEvent shippingGracePeriodExpiredDomainEvent = new(orderId);

            // Act
            processManager.Transition(shippingGracePeriodExpiredDomainEvent, clockService.Now);
            
            // Assert
            processManager.Outbox.Messages.Count.Should().Be(expectedMessagesCount);
            processManager.Status.Should().Be(OrderingProcessManagerStatus.OrderShipped);
        }
    }
}