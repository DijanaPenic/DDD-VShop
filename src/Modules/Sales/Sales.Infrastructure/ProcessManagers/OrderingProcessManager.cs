using NodaTime;
using System.Linq;

using VShop.Modules.Sales.Domain.Events;
using VShop.Modules.Sales.Domain.Events.Reminders;
using VShop.Modules.Sales.Infrastructure.Commands;
using VShop.Modules.Billing.Integration.Events;
using VShop.Modules.Catalog.Integration.Events;
using VShop.SharedKernel.Domain.ValueObjects;
using VShop.SharedKernel.EventSourcing.ProcessManagers;
using VShop.SharedKernel.Infrastructure.Messaging;
using VShop.SharedKernel.Infrastructure.Events.Contracts;

// TODO - email alert support.
namespace VShop.Modules.Sales.Infrastructure.ProcessManagers
{
    internal class OrderingProcessManager : ProcessManager
    {
        public EntityId ShoppingCartId { get; private set; }
        public EntityId OrderId { get; private set; }
        public OrderingProcessManagerStatus Status { get; private set; }
        
        public OrderingProcessManager()
        {
            RegisterEvent<ShoppingCartCheckoutRequestedDomainEvent>(Handle);
            RegisterEvent<OrderPlacedDomainEvent>(Handle);
            RegisterEvent<PaymentGracePeriodExpiredDomainEvent>(Handle);
            RegisterEvent<PaymentSucceededIntegrationEvent>(Handle);
            RegisterEvent<OrderStatusSetToPaidDomainEvent>(Handle);
            RegisterEvent<OrderStockProcessingGracePeriodExpiredDomainEvent>(Handle);
            RegisterEvent<OrderStockProcessedIntegrationEvent>(Handle);
            RegisterEvent<OrderStatusSetToPendingShippingDomainEvent>(Handle);
            RegisterEvent<ShippingGracePeriodExpiredDomainEvent>(Handle);
        }

        private void Handle(ShoppingCartCheckoutRequestedDomainEvent @event, Instant _) 
            => RaiseCommand(new PlaceOrderCommand(OrderId, ShoppingCartId, new MessageMetadata()));

        private void Handle(OrderPlacedDomainEvent @event, Instant now)
        {
            RaiseCommand(new DeleteShoppingCartCommand(ShoppingCartId, new MessageMetadata()));
            
            // Schedule a reminder for payment.
            ScheduleReminder
            (
                new PaymentGracePeriodExpiredDomainEvent(OrderId),
                now.Plus(Duration.FromMinutes(Settings.PaymentGracePeriodInMinutes))
            );
        }
        
        private void Handle(PaymentGracePeriodExpiredDomainEvent @event, Instant _)
        {
            // User didn't manage to pay so we need to cancel the order.
            if (Status is OrderingProcessManagerStatus.OrderPaymentFailed)
                RaiseCommand(new CancelOrderCommand(OrderId, new MessageMetadata()));
            
            // Send an alert for the OrderPlaced status. The payment department
            // never replied with PaymentSucceededIntegrationEvent or PaymentFailedIntegrationEvent.
        }

        private void Handle(PaymentSucceededIntegrationEvent @event, Instant now)
            => RaiseCommand(new SetPaidOrderStatusCommand(OrderId, new MessageMetadata()));

        private void Handle(OrderStatusSetToPaidDomainEvent @event, Instant now)
        {
            // Schedule a reminder for stock confirmation.
            ScheduleReminder
            (
                new OrderStockProcessingGracePeriodExpiredDomainEvent(OrderId),
                now.Plus(Duration.FromMinutes(Settings.StockConfirmationGracePeriodInMinutes))
            );
        }
        
        private void Handle(OrderStockProcessingGracePeriodExpiredDomainEvent @event, Instant _)
        {
            if (Status is OrderingProcessManagerStatus.OrderStockConfirmed) return;
            
            // Send an alert. Catalog never replied so there is no way to know if we can
            // move forward with refund and/or the shipping process.
        }
        
        private void Handle(OrderStockProcessedIntegrationEvent @event, Instant now)
        {
            RaiseCommand(new FinalizeOrderCommand
            (
                OrderId,
                @event.OrderLines.Select(ol => new FinalizeOrderCommand.Types.OrderLine
                (
                    ol.ProductId,
                    ol.OutOfStockQuantity
                )),
                new MessageMetadata()
            ));
        }
        
        private void Handle(OrderStatusSetToPendingShippingDomainEvent @event, Instant now)
        {
            // Schedule a reminder for shipping.
            ScheduleReminder
            (
                new ShippingGracePeriodExpiredDomainEvent(OrderId),
                now.Plus(Duration.FromHours(Settings.ShippingGracePeriodInHours))
            );
        }
        
        private void Handle(ShippingGracePeriodExpiredDomainEvent @event, Instant _)
        {
            // The order is already shipped so there is nothing to address.
            if (Status is OrderingProcessManagerStatus.OrderShipped) return;

            // Send an alert. The shipping department never replied.
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
                case OrderStockProcessedIntegrationEvent _:
                    Status = OrderingProcessManagerStatus.OrderStockConfirmed;
                    break;
                case OrderStatusSetToPendingShippingDomainEvent _:
                    Status = OrderingProcessManagerStatus.OrderPendingShipping;
                    break;
                case OrderStatusSetToShippedDomainEvent _:
                    Status = OrderingProcessManagerStatus.OrderShipped;
                    break;
            }
        }
        
        public static class Settings
        {
            public const int StockConfirmationGracePeriodInMinutes = 2;
            public const int PaymentGracePeriodInMinutes = 30;
            public const int ShippingGracePeriodInHours = 24;
        }
    }
}