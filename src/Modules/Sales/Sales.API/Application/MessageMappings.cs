using VShop.SharedKernel.Messaging;

using VShop.Modules.Sales.Domain.Events;
using VShop.Modules.Sales.Integration.Events;
using VShop.Modules.Sales.API.Application.Commands;
using VShop.Modules.Billing.Integration.Events;
using VShop.Modules.Catalog.Integration.Events;

using static VShop.SharedKernel.Messaging.MessageTypeMapper;

namespace VShop.Modules.Sales.API.Application
{
    public static class MessageMappings
    {
        public static void MapMessageTypes()
        {
            // Configure domain events
            AddCustomMap<ShoppingCartCreatedDomainEvent>(nameof(ShoppingCartCreatedDomainEvent));
            AddCustomMap<ShoppingCartCheckoutRequestedDomainEvent>(nameof(ShoppingCartCheckoutRequestedDomainEvent));
            AddCustomMap<ShoppingCartDeletionRequestedDomainEvent>(nameof(ShoppingCartDeletionRequestedDomainEvent));
            AddCustomMap<ShoppingCartItemQuantityDecreasedDomainEvent>(nameof(ShoppingCartItemQuantityDecreasedDomainEvent));
            AddCustomMap<ShoppingCartItemQuantityIncreasedDomainEvent>(nameof(ShoppingCartItemQuantityIncreasedDomainEvent));
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

            // Configure integration events - local
            AddCustomMap<OrderStatusSetToPaidIntegrationEvent>(nameof(OrderStatusSetToPaidIntegrationEvent));
            
            // Configure integration events - remote
            AddCustomMap<PaymentFailedIntegrationEvent>(nameof(PaymentFailedIntegrationEvent));
            AddCustomMap<PaymentSucceededIntegrationEvent>(nameof(PaymentSucceededIntegrationEvent));
            AddCustomMap<OrderStockConfirmedIntegrationEvent>(nameof(OrderStockConfirmedIntegrationEvent));
            
            // Configure commands
            AddCustomMap<PlaceOrderCommand>(nameof(PlaceOrderCommand));
            AddCustomMap<DeleteShoppingCartCommand>(nameof(DeleteShoppingCartCommand));
            AddCustomMap<CancelOrderCommand>(nameof(CancelOrderCommand));

            // Configure scheduled message
            AddCustomMap<ScheduledMessage>(nameof(ScheduledMessage));
        }
    }
}