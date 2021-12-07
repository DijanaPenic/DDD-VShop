using System;
using Newtonsoft.Json;

using VShop.Modules.Sales.Domain.Events;
using VShop.Modules.Sales.API.Application.Commands;
using VShop.Modules.Billing.Integration.Events;
using VShop.SharedKernel.Messaging.Events;
using VShop.SharedKernel.EventSourcing.ProcessManagers;

namespace VShop.Modules.Sales.API.Application.ProcessManagers
{
    internal class OrderingProcessManager : ProcessManager
    {
        public Guid ShoppingCartId { get; private set; }
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

        public void Handle(ShoppingCartCheckoutRequestedDomainEvent _)
        {
            PlaceOrderCommand placeOrderCommand = new()
            {
                OrderId = Id,
                ShoppingCartId = ShoppingCartId
            };
            RaiseCommand(placeOrderCommand);
        }

        public void Handle(OrderPlacedDomainEvent _)
        {
            DeleteShoppingCartCommand deleteShoppingCartCommand = new(ShoppingCartId);
            RaiseCommand(deleteShoppingCartCommand);
        }
        
        public void Handle(PaymentSucceededIntegrationEvent @event)
            => ScheduleDomainEvent
            (
                new ShippingGracePeriodExpiredDomainEvent(Id, JsonConvert.SerializeObject(@event)),
                DateTime.UtcNow.AddHours(24)
            );
        
        public void Handle(ShippingGracePeriodExpiredDomainEvent @event)
        {
            // The order is already shipped so there is nothing to address.
            if (Status is OrderingProcessManagerStatus.OrderShipped) return;

            // Cancelling the order as we already escalated the order request two times. Better alternative would be
            // to send out an email to the support team as we need to report a problem with the shipping department.
            if (ShippingCheckCount >= 3)
            {
                CancelOrderCommand cancelOrderCommand = new(Id);
                RaiseCommand(cancelOrderCommand);
            }
            // Resend the payment status message to the shipping department (kind of a reminder message).
            else
            {
                RaiseIntegrationEvent(JsonConvert.DeserializeObject<PaymentSucceededIntegrationEvent>(@event.Content));
            }
        }

        public void Handle(PaymentFailedIntegrationEvent _)
            => ScheduleDomainEvent
            (
                new PaymentGracePeriodExpiredDomainEvent(Id),
                DateTime.UtcNow.AddMinutes(30)
            );
        
        public void Handle(PaymentGracePeriodExpiredDomainEvent _)
        {
            // User managed to successfully pay the order.
            if (Status is not OrderingProcessManagerStatus.OrderPaymentFailed) return;

            // User didn't manage to pay so we need to cancel the order.
            CancelOrderCommand cancelOrderCommand = new(Id);
            RaiseCommand(cancelOrderCommand);
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
    }
}