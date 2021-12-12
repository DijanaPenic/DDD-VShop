using System;
using NodaTime;

using VShop.Modules.Sales.Domain.Events;
using VShop.Modules.Sales.API.Application.Commands;
using VShop.Modules.Billing.Integration.Events;
using VShop.SharedKernel.Messaging.Events;
using VShop.SharedKernel.EventSourcing.ProcessManagers;
using VShop.SharedKernel.Infrastructure.Services.Contracts;

namespace VShop.Modules.Sales.API.Application.ProcessManagers
{
    public class OrderingProcessManager : ProcessManager
    {
        public Guid ShoppingCartId { get; private set; }
        public OrderingProcessManagerStatus Status { get; private set; }
        public int ShippingCheckCount { get; private set; }

        public OrderingProcessManager(IClockService clockService): base(clockService)
        {
            RegisterEvent<ShoppingCartCheckoutRequestedDomainEvent>(Handle);
            RegisterEvent<OrderPlacedDomainEvent>(Handle);
            RegisterEvent<PaymentSucceededIntegrationEvent>(Handle);
            RegisterEvent<PaymentFailedIntegrationEvent>(Handle);
            RegisterEvent<ShippingGracePeriodExpiredDomainEvent>(Handle);
            RegisterEvent<PaymentGracePeriodExpiredDomainEvent>(Handle);
        }

        public void Handle(ShoppingCartCheckoutRequestedDomainEvent @event) 
            => RaiseCommand(new PlaceOrderCommand
            {
                OrderId = Id,
                ShoppingCartId = ShoppingCartId
            });

        public void Handle(OrderPlacedDomainEvent @event) 
            => RaiseCommand(new DeleteShoppingCartCommand(ShoppingCartId));

        public void Handle(PaymentSucceededIntegrationEvent @event)
            => ScheduleDomainEvent
            (
                new ShippingGracePeriodExpiredDomainEvent(Id, @event),
                ClockService.Now.Plus(Duration.FromHours(Settings.ShippingGracePeriodInHours))
            );
        
        public void Handle(ShippingGracePeriodExpiredDomainEvent @event)
        {
            // The order is already shipped so there is nothing to address.
            if (Status is OrderingProcessManagerStatus.OrderShipped) return;

            // Cancelling the order as we already escalated the order request two times. Better alternative would be
            // to send out an email to the support team as we need to report a problem with the shipping department.
            if (ShippingCheckCount >= Settings.ShippingCheckThreshold)
            {
                RaiseCommand(new CancelOrderCommand(Id));
            }
            // Resend the payment status message to the shipping department (kind of a reminder message).
            else
            {
                RaiseIntegrationEvent<PaymentSucceededIntegrationEvent>(@event.Content);
            }
        }

        // TODO - can these methods be private? They can't be used outside!
        public void Handle(PaymentFailedIntegrationEvent @event)
            => ScheduleDomainEvent
            (
                new PaymentGracePeriodExpiredDomainEvent(Id),
                ClockService.Now.Plus(Duration.FromMinutes(Settings.PaymentGracePeriodInMinutes))
            );
        
        public void Handle(PaymentGracePeriodExpiredDomainEvent @event)
        {
            // User managed to successfully pay the order - nothing to do.
            if (Status is not OrderingProcessManagerStatus.OrderPaymentFailed) return;

            // User didn't manage to pay so we need to cancel the order.
            RaiseCommand(new CancelOrderCommand(Id));
        }

        protected override void ApplyEvent(IBaseEvent @event)
        {
            switch (@event)
            {
                case ShoppingCartCheckoutRequestedDomainEvent e:
                    Id = e.OrderId;
                    ShoppingCartId = e.ShoppingCartId;
                    Status = OrderingProcessManagerStatus.CheckoutRequested;
                    break;
                case OrderPlacedDomainEvent _:
                    Status = OrderingProcessManagerStatus.OrderPlaced;
                    break;
                case PaymentSucceededIntegrationEvent _:
                    Status = OrderingProcessManagerStatus.OrderPaymentSucceeded;
                    break;
                case PaymentFailedIntegrationEvent _:
                    Status = OrderingProcessManagerStatus.OrderPaymentFailed;
                    break;
                case OrderStatusSetToCancelledDomainEvent _:
                    Status = OrderingProcessManagerStatus.OrderCancelled;
                    break;
                case ShippingGracePeriodExpiredDomainEvent _:
                    ShippingCheckCount++;
                    break;
            }
        }
        
        public static class Settings
        {
            public const int ShippingCheckThreshold = 3;
            public const int PaymentGracePeriodInMinutes = 30;
            public const int ShippingGracePeriodInHours = 24;
        }
    }
}