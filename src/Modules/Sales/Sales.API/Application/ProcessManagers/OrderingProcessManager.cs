using NodaTime;

using VShop.Modules.Sales.Domain.Events;
using VShop.Modules.Sales.API.Application.Commands;
using VShop.Modules.Billing.Integration.Events;
using VShop.SharedKernel.Domain.ValueObjects;
using VShop.SharedKernel.Messaging.Events;
using VShop.SharedKernel.EventSourcing.ProcessManagers;

namespace VShop.Modules.Sales.API.Application.ProcessManagers
{
    public class OrderingProcessManager : ProcessManager
    {
        public EntityId ShoppingCartId { get; private set; }
        public EntityId OrderId { get; private set; }
        public OrderingProcessManagerStatus Status { get; private set; }
        public int ShippingCheckCount { get; private set; }
        
        public OrderingProcessManager()
        {
            RegisterEvent<ShoppingCartCheckoutRequestedDomainEvent>(Handle);
            RegisterEvent<OrderPlacedDomainEvent>(Handle);
            RegisterEvent<PaymentSucceededIntegrationEvent>(Handle);
            RegisterEvent<PaymentFailedIntegrationEvent>(Handle);
            RegisterEvent<ShippingGracePeriodExpiredDomainEvent>(Handle);
            RegisterEvent<PaymentGracePeriodExpiredDomainEvent>(Handle);
        }

        private void Handle(ShoppingCartCheckoutRequestedDomainEvent @event) 
            => RaiseCommand(new PlaceOrderCommand
            {
                OrderId = OrderId,
                ShoppingCartId = ShoppingCartId
            });

        private void Handle(OrderPlacedDomainEvent @event) 
            => RaiseCommand(new DeleteShoppingCartCommand(ShoppingCartId));

        private void Handle(PaymentSucceededIntegrationEvent @event, Instant now)
            => ScheduleDomainEvent
            (
                new ShippingGracePeriodExpiredDomainEvent(OrderId, @event),
                now.Plus(Duration.FromHours(Settings.ShippingGracePeriodInHours))
            );
        
        private void Handle(ShippingGracePeriodExpiredDomainEvent @event)
        {
            // The order is already shipped so there is nothing to address.
            if (Status is OrderingProcessManagerStatus.OrderShipped) return;

            // Cancelling the order as we already escalated the order request two times. Better alternative would be
            // to send out an email to the support team as we need to report a problem with the shipping department.
            if (ShippingCheckCount >= Settings.ShippingCheckThreshold)
            {
                RaiseCommand(new CancelOrderCommand(OrderId));
            }
            // Resend the payment status message to the shipping department (kind of a reminder message).
            else
            {
                RaiseIntegrationEvent<PaymentSucceededIntegrationEvent>(@event.Content);
            }
        }
        
        private void Handle(PaymentFailedIntegrationEvent @event, Instant now)
            => ScheduleDomainEvent
            (
                new PaymentGracePeriodExpiredDomainEvent(Id),
                now.Plus(Duration.FromMinutes(Settings.PaymentGracePeriodInMinutes))
            );
        
        private void Handle(PaymentGracePeriodExpiredDomainEvent @event)
        {
            // User didn't manage to pay so we need to cancel the order.
            if (Status is OrderingProcessManagerStatus.OrderPaymentFailed) 
                RaiseCommand(new CancelOrderCommand(OrderId));
        }

        protected override void ApplyEvent(IBaseEvent @event)
        {
            switch (@event)
            {
                case ShoppingCartCheckoutRequestedDomainEvent e:
                    Id = e.OrderId;
                    OrderId = EntityId.Create(e.OrderId).Data;
                    ShoppingCartId = EntityId.Create(e.ShoppingCartId).Data;
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