using VShop.Modules.Sales.Domain.Events;
using VShop.Modules.Sales.Domain.Events.Reminders;
using VShop.Modules.Sales.Core.Commands;
using VShop.Modules.Sales.Integration.Events;
using VShop.Modules.Billing.Integration.Events;
using VShop.Modules.Catalog.Integration.Events;
using VShop.SharedKernel.Infrastructure.Messaging;

using static VShop.SharedKernel.Infrastructure.Messaging.MessageTypeMapper;

namespace VShop.Modules.Sales.API.Application
{
    public static class MessageMappings
    {
        public static void Initialize()
        {
            MapMessageTypes();
            MapMessageTransformations();
        }
            
        private static void MapMessageTypes()
        {
            // Configure domain events
            AddCustomMap<ShoppingCartProductPriceChangedDomainEvent>(nameof(ShoppingCartProductPriceChangedDomainEvent));
            AddCustomMap<ShoppingCartCreatedDomainEvent>(nameof(ShoppingCartCreatedDomainEvent));
            AddCustomMap<ShoppingCartCheckoutRequestedDomainEvent>(nameof(ShoppingCartCheckoutRequestedDomainEvent));
            AddCustomMap<ShoppingCartDeletedDomainEvent>(nameof(ShoppingCartDeletedDomainEvent));
            AddCustomMap<ShoppingCartProductQuantityDecreasedDomainEvent>(nameof(ShoppingCartProductQuantityDecreasedDomainEvent));
            AddCustomMap<ShoppingCartProductQuantityIncreasedDomainEvent>(nameof(ShoppingCartProductQuantityIncreasedDomainEvent));
            AddCustomMap<ShoppingCartContactInformationSetDomainEvent>(nameof(ShoppingCartContactInformationSetDomainEvent));
            AddCustomMap<ShoppingCartDeliveryAddressSetDomainEvent>(nameof(ShoppingCartDeliveryAddressSetDomainEvent));
            AddCustomMap<ShoppingCartDeliveryCostChangedDomainEvent>(nameof(ShoppingCartDeliveryCostChangedDomainEvent));
            AddCustomMap<ShoppingCartProductAddedDomainEvent>(nameof(ShoppingCartProductAddedDomainEvent));
            AddCustomMap<ShoppingCartProductRemovedDomainEvent>(nameof(ShoppingCartProductRemovedDomainEvent));
            AddCustomMap<OrderStatusSetToCancelledDomainEvent>(nameof(OrderStatusSetToCancelledDomainEvent));
            AddCustomMap<OrderStatusSetToShippedDomainEvent>(nameof(OrderStatusSetToShippedDomainEvent));
            AddCustomMap<OrderLineAddedDomainEvent>(nameof(OrderLineAddedDomainEvent));
            AddCustomMap<OrderPlacedDomainEvent>(nameof(OrderPlacedDomainEvent));
            AddCustomMap<PaymentGracePeriodExpiredDomainEvent>(nameof(PaymentGracePeriodExpiredDomainEvent));
            AddCustomMap<ShippingGracePeriodExpiredDomainEvent>(nameof(ShippingGracePeriodExpiredDomainEvent));
            AddCustomMap<OrderStatusSetToPaidDomainEvent>(nameof(OrderStatusSetToPaidDomainEvent));
            AddCustomMap<OrderLineOutOfStockRemovedDomainEvent>(nameof(OrderLineOutOfStockRemovedDomainEvent));
            AddCustomMap<OrderStatusSetToPendingShippingDomainEvent>(nameof(OrderStatusSetToPendingShippingDomainEvent));
            AddCustomMap<OrderStockProcessingGracePeriodExpiredDomainEvent>(nameof(OrderStockProcessingGracePeriodExpiredDomainEvent));
            AddCustomMap<SetPaidOrderStatusCommand>(nameof(SetPaidOrderStatusCommand));
            AddCustomMap<FinalizeOrderCommand>(nameof(FinalizeOrderCommand));
            
            // Configure integration events - local
            AddCustomMap<OrderStatusSetToPaidIntegrationEvent>(nameof(OrderStatusSetToPaidIntegrationEvent));
            AddCustomMap<OrderFinalizedIntegrationEvent>(nameof(OrderFinalizedIntegrationEvent));
            
            // Configure integration events - remote
            AddCustomMap<PaymentFailedIntegrationEvent>(nameof(PaymentFailedIntegrationEvent));
            AddCustomMap<PaymentSucceededIntegrationEvent>(nameof(PaymentSucceededIntegrationEvent));
            AddCustomMap<OrderStockProcessedIntegrationEvent>(nameof(OrderStockProcessedIntegrationEvent));
            
            // Configure commands
            AddCustomMap<PlaceOrderCommand>(nameof(PlaceOrderCommand));
            AddCustomMap<DeleteShoppingCartCommand>(nameof(DeleteShoppingCartCommand));
            AddCustomMap<CancelOrderCommand>(nameof(CancelOrderCommand));

            // Configure scheduled message
            AddCustomMap<ScheduledMessage>(nameof(ScheduledMessage));
        }

        private static void MapMessageTransformations()
        {
            // CAUTION: this is just a showcase example.
            //Register<ShoppingCartCreatedDomainEvent, ShoppingCartCreatedDomainEvent2>(ShoppingCartCreatedDomainEvent2.ConvertFrom);
        }
    }
}