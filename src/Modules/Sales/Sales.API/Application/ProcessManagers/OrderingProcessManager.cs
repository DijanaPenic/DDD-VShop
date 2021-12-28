using NodaTime;
using System.Linq;

using VShop.Modules.Sales.Domain.Events;
using VShop.Modules.Sales.API.Application.Commands;
using VShop.Modules.Billing.Integration.Events;
using VShop.Modules.Catalog.Integration.Events;
using VShop.SharedKernel.Messaging.Events;
using VShop.SharedKernel.Domain.ValueObjects;
using VShop.SharedKernel.EventSourcing.ProcessManagers;

// TODO - email alert support
namespace VShop.Modules.Sales.API.Application.ProcessManagers
{
    public class OrderingProcessManager : ProcessManager
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
            RegisterEvent<StockConfirmationGracePeriodExpiredDomainEvent>(Handle);
            RegisterEvent<OrderStockConfirmedIntegrationEvent>(Handle);
            RegisterEvent<OrderStatusSetToPendingShippingDomainEvent>(Handle);
            RegisterEvent<ShippingGracePeriodExpiredDomainEvent>(Handle);
        }

        private void Handle(ShoppingCartCheckoutRequestedDomainEvent @event) 
            => RaiseCommand(new PlaceOrderCommand
            {
                OrderId = OrderId,
                ShoppingCartId = ShoppingCartId
            });

        private void Handle(OrderPlacedDomainEvent @event, Instant now)
        {
            RaiseCommand(new DeleteShoppingCartCommand(ShoppingCartId));
            
            // Schedule a reminder for payment.
            ScheduleReminder
            (
                new PaymentGracePeriodExpiredDomainEvent(OrderId),
                now.Plus(Duration.FromMinutes(Settings.PaymentGracePeriodInMinutes))
            );
        }
        
        private void Handle(PaymentGracePeriodExpiredDomainEvent @event)
        {
            // User didn't manage to pay so we need to cancel the order.
            if (Status is OrderingProcessManagerStatus.OrderPaymentFailed)
                RaiseCommand(new CancelOrderCommand(OrderId));
            
            // Send an alert for the OrderPlaced status. The payment department
            // never replied with PaymentSucceededIntegrationEvent or PaymentFailedIntegrationEvent.
        }

        private void Handle(PaymentSucceededIntegrationEvent @event, Instant now)
            => RaiseCommand(new SetPaidOrderStatusCommand(OrderId));
        
        private void Handle(OrderStatusSetToPaidDomainEvent @event, Instant now)
        {
            // Schedule a reminder for stock confirmation.
            ScheduleReminder
            (
                new StockConfirmationGracePeriodExpiredDomainEvent(OrderId),
                now.Plus(Duration.FromMinutes(Settings.StockConfirmationGracePeriodInMinutes))
            );
        }
        
        private void Handle(StockConfirmationGracePeriodExpiredDomainEvent @event)
        {
            if (Status is OrderingProcessManagerStatus.OrderStockConfirmed) return;
            
            // Send an alert. Catalog never replied so there is no way to know if we can
            // move forward with refund and/or the shipping process.
        }
        
        private void Handle(OrderStockConfirmedIntegrationEvent @event, Instant now)
        {
            RaiseCommand(new FinalizeOrderCommand
            (
                OrderId,
                @event.OrderLines.Select(ol => new FinalizeOrderCommand.OrderLine
                {
                    ProductId = ol.ProductId,
                    OutOfStockQuantity = ol.OutOfStockQuantity,
                    EnoughStock = ol.EnoughStock
                }).ToList()
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

        private void Handle(ShippingGracePeriodExpiredDomainEvent @event)
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
                case OrderStockConfirmedIntegrationEvent _:
                    Status = OrderingProcessManagerStatus.OrderStockConfirmed;
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