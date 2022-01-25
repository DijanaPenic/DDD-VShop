using VShop.Modules.Sales.Domain.Events;
using VShop.Modules.Sales.Domain.Events.Reminders;
using VShop.Modules.Sales.Integration.Events;
using VShop.Modules.Sales.Infrastructure.Commands;
using VShop.Modules.Billing.Integration.Events;
using VShop.Modules.Catalog.Integration.Events;
using VShop.SharedKernel.Infrastructure.Messaging;
using VShop.SharedKernel.Infrastructure.Messaging.Contracts;

namespace VShop.Modules.Sales.Infrastructure.Configuration
{
    internal static class SalesMessageRegistry
    {
        public static IMessageRegistry Initialize()
        {
            MessageRegistry registry = new();

            registry.RegisterMessages();
            registry.RegisterTransformations();

            return registry;
        }
        
        public static IMessageRegistry RegisterMessages(this MessageRegistry registry)
        {
            // Configure domain events
            registry.Add<ShoppingCartProductPriceChangedDomainEvent>(nameof(ShoppingCartProductPriceChangedDomainEvent));
            registry.Add<ShoppingCartCreatedDomainEvent>(nameof(ShoppingCartCreatedDomainEvent));
            registry.Add<ShoppingCartCheckoutRequestedDomainEvent>(nameof(ShoppingCartCheckoutRequestedDomainEvent));
            registry.Add<ShoppingCartDeletedDomainEvent>(nameof(ShoppingCartDeletedDomainEvent));
            registry.Add<ShoppingCartProductQuantityDecreasedDomainEvent>(nameof(ShoppingCartProductQuantityDecreasedDomainEvent));
            registry.Add<ShoppingCartProductQuantityIncreasedDomainEvent>(nameof(ShoppingCartProductQuantityIncreasedDomainEvent));
            registry.Add<ShoppingCartContactInformationSetDomainEvent>(nameof(ShoppingCartContactInformationSetDomainEvent));
            registry.Add<ShoppingCartDeliveryAddressSetDomainEvent>(nameof(ShoppingCartDeliveryAddressSetDomainEvent));
            registry.Add<ShoppingCartDeliveryCostChangedDomainEvent>(nameof(ShoppingCartDeliveryCostChangedDomainEvent));
            registry.Add<ShoppingCartProductAddedDomainEvent>(nameof(ShoppingCartProductAddedDomainEvent));
            registry.Add<ShoppingCartProductRemovedDomainEvent>(nameof(ShoppingCartProductRemovedDomainEvent));
            registry.Add<OrderStatusSetToCancelledDomainEvent>(nameof(OrderStatusSetToCancelledDomainEvent));
            registry.Add<OrderStatusSetToShippedDomainEvent>(nameof(OrderStatusSetToShippedDomainEvent));
            registry.Add<OrderLineAddedDomainEvent>(nameof(OrderLineAddedDomainEvent));
            registry.Add<OrderPlacedDomainEvent>(nameof(OrderPlacedDomainEvent));
            registry.Add<PaymentGracePeriodExpiredDomainEvent>(nameof(PaymentGracePeriodExpiredDomainEvent));
            registry.Add<ShippingGracePeriodExpiredDomainEvent>(nameof(ShippingGracePeriodExpiredDomainEvent));
            registry.Add<OrderStatusSetToPaidDomainEvent>(nameof(OrderStatusSetToPaidDomainEvent));
            registry.Add<OrderLineOutOfStockRemovedDomainEvent>(nameof(OrderLineOutOfStockRemovedDomainEvent));
            registry.Add<OrderStatusSetToPendingShippingDomainEvent>(nameof(OrderStatusSetToPendingShippingDomainEvent));
            registry.Add<OrderStockProcessingGracePeriodExpiredDomainEvent>(nameof(OrderStockProcessingGracePeriodExpiredDomainEvent));
            registry.Add<SetPaidOrderStatusCommand>(nameof(SetPaidOrderStatusCommand));
            registry.Add<FinalizeOrderCommand>(nameof(FinalizeOrderCommand));
            
            // Configure integration events - local
            registry.Add<OrderStatusSetToPaidIntegrationEvent>(nameof(OrderStatusSetToPaidIntegrationEvent));
            registry.Add<OrderFinalizedIntegrationEvent>(nameof(OrderFinalizedIntegrationEvent));
            
            // Configure integration events - remote
            registry.Add<PaymentFailedIntegrationEvent>(nameof(PaymentFailedIntegrationEvent));
            registry.Add<PaymentSucceededIntegrationEvent>(nameof(PaymentSucceededIntegrationEvent));
            registry.Add<OrderStockProcessedIntegrationEvent>(nameof(OrderStockProcessedIntegrationEvent));
            
            // Configure commands
            registry.Add<PlaceOrderCommand>(nameof(PlaceOrderCommand));
            registry.Add<DeleteShoppingCartCommand>(nameof(DeleteShoppingCartCommand));
            registry.Add<CancelOrderCommand>(nameof(CancelOrderCommand));

            // Configure scheduled message
            registry.Add<ScheduledMessage>(nameof(ScheduledMessage));

            return registry;
        }
        
        private static IMessageRegistry RegisterTransformations(this MessageRegistry registry)
        {
            // CAUTION: this is just a showcase example.
            //registry.Register<ShoppingCartCreatedDomainEvent, ShoppingCartCreatedDomainEvent2>(ShoppingCartCreatedDomainEvent2.ConvertFrom);

            return registry;
        }
    }
}