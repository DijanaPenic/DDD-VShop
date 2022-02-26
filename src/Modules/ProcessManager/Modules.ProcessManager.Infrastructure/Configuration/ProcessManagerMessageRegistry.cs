using VShop.Modules.Billing.Integration.Events;
using VShop.Modules.Catalog.Integration.Events;
using VShop.Modules.ProcessManager.Infrastructure.Messages.Commands;
using VShop.Modules.ProcessManager.Infrastructure.Messages.Events;
using VShop.Modules.ProcessManager.Infrastructure.Messages.Reminders;
using VShop.SharedKernel.Infrastructure.Messaging;
using VShop.SharedKernel.Infrastructure.Messaging.Contracts;

namespace VShop.Modules.ProcessManager.Infrastructure.Configuration
{
    internal static class ProcessManagerMessageRegistry
    {
        public static IMessageRegistry Initialize()
        {
            MessageRegistry registry = new();

            registry.RegisterMessages();
            registry.RegisterTransformations();

            return registry;
        }
        
        private static void RegisterMessages(this MessageRegistry registry)
        {
            // Configure reminders
            registry.Add<OrderStockProcessingGracePeriodExpiredDomainEvent>(nameof(OrderStockProcessingGracePeriodExpiredDomainEvent));
            registry.Add<PaymentGracePeriodExpiredDomainEvent>(nameof(PaymentGracePeriodExpiredDomainEvent));
            registry.Add<ShippingGracePeriodExpiredDomainEvent>(nameof(ShippingGracePeriodExpiredDomainEvent));
            
            // Configure domain events
            registry.Add<ShoppingCartCheckoutRequestedDomainEvent>(nameof(ShoppingCartCheckoutRequestedDomainEvent));
            registry.Add<OrderStatusSetToCancelledDomainEvent>(nameof(OrderStatusSetToCancelledDomainEvent));
            registry.Add<OrderStatusSetToShippedDomainEvent>(nameof(OrderStatusSetToShippedDomainEvent));
            registry.Add<OrderPlacedDomainEvent>(nameof(OrderPlacedDomainEvent));
            registry.Add<OrderStatusSetToPaidDomainEvent>(nameof(OrderStatusSetToPaidDomainEvent));
            registry.Add<OrderStatusSetToPendingShippingDomainEvent>(nameof(OrderStatusSetToPendingShippingDomainEvent));

            // Configure integration events - remote
            registry.Add<PaymentFailedIntegrationEvent>(nameof(PaymentFailedIntegrationEvent));
            registry.Add<PaymentSucceededIntegrationEvent>(nameof(PaymentSucceededIntegrationEvent));
            registry.Add<OrderStockProcessedIntegrationEvent>(nameof(OrderStockProcessedIntegrationEvent));
            
            // Configure commands
            registry.Add<PlaceOrderCommand>(nameof(PlaceOrderCommand));
            registry.Add<DeleteShoppingCartCommand>(nameof(DeleteShoppingCartCommand));
            registry.Add<CancelOrderCommand>(nameof(CancelOrderCommand));
            registry.Add<SetPaidOrderStatusCommand>(nameof(SetPaidOrderStatusCommand));
            registry.Add<FinalizeOrderCommand>(nameof(FinalizeOrderCommand));

            // Configure scheduled message
            registry.Add<ScheduledMessage>(nameof(ScheduledMessage));
        }
        
        private static void RegisterTransformations(this MessageRegistry registry)
        {

        }
    }
}