using VShop.Modules.Sales.Domain.Events;
using VShop.Modules.Sales.Integration.Events;

using static VShop.SharedKernel.EventSourcing.Messaging.MessageTypeMapper;

namespace VShop.Modules.Sales.Infrastructure
{
    public static class MessageMappings
    {
        public static void MapEventTypes()
        {
            // Configure domain events
            Map<ShoppingCartCreatedDomainEvent>(nameof(ShoppingCartCreatedDomainEvent));
            Map<ShoppingCartCheckoutRequestedDomainEvent>(nameof(ShoppingCartCheckoutRequestedDomainEvent));
            Map<ShoppingCartDeletionRequestedDomainEvent>(nameof(ShoppingCartDeletionRequestedDomainEvent));
            Map<ShoppingCartItemQuantityDecreasedDomainEvent>(nameof(ShoppingCartItemQuantityDecreasedDomainEvent));
            Map<ShoppingCartItemQuantityIncreasedDomainEvent>(nameof(ShoppingCartItemQuantityIncreasedDomainEvent));
            Map<ShoppingCartContactInformationSetDomainEvent>(nameof(ShoppingCartContactInformationSetDomainEvent));
            Map<ShoppingCartDeliveryAddressSetDomainEvent>(nameof(ShoppingCartDeliveryAddressSetDomainEvent));
            Map<ShoppingCartDeliveryCostChangedDomainEvent>(nameof(ShoppingCartDeliveryCostChangedDomainEvent));
            Map<ShoppingCartProductAddedDomainEvent>(nameof(ShoppingCartProductAddedDomainEvent));
            Map<ShoppingCartProductRemovedDomainEvent>(nameof(ShoppingCartProductRemovedDomainEvent));
            Map<OrderStatusSetToCancelledDomainEvent>(nameof(OrderStatusSetToCancelledDomainEvent));
            Map<OrderStatusSetToShippedDomainEvent>(nameof(OrderStatusSetToShippedDomainEvent));
            Map<OrderPlacedDomainEvent>(nameof(OrderPlacedDomainEvent));
            Map<OrderItemAddedDomainEvent>(nameof(OrderItemAddedDomainEvent));
            
            // Configure integration events
            Map<OrderPlacedIntegrationEvent>(nameof(OrderPlacedIntegrationEvent));
        }
    }
}