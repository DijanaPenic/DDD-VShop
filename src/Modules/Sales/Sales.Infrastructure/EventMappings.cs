using VShop.Modules.Sales.Domain.Events;
using VShop.Modules.Sales.Integration.Events;

using static VShop.SharedKernel.EventSourcing.EventTypeMapper;

namespace VShop.Modules.Sales.Infrastructure
{
    public static class EventMappings
    {
        public static void MapEventTypes()
        {
            // Configure domain events
            Map<ShoppingCartCreatedDomainEvent>(nameof(ShoppingCartCreatedDomainEvent));
            Map<ShoppingCartCheckoutRequestedDomainEvent>(nameof(ShoppingCartCheckoutRequestedDomainEvent));
            Map<ShoppingCartDeletionRequestedDomainEvent>(nameof(ShoppingCartDeletionRequestedDomainEvent));
            Map<ShoppingCartItemQuantityDecreasedDomainEvent>(nameof(ShoppingCartItemQuantityDecreasedDomainEvent));
            Map<ShoppingCartItemQuantityIncreasedDomainEvent>(nameof(ShoppingCartItemQuantityIncreasedDomainEvent));
            Map<ContactInformationSetDomainEvent>(nameof(ContactInformationSetDomainEvent));
            Map<DeliveryAddressSetDomainEvent>(nameof(DeliveryAddressSetDomainEvent));
            Map<DeliveryCostChangedDomainEvent>(nameof(DeliveryCostChangedDomainEvent));
            Map<ProductAddedToShoppingCartDomainEvent>(nameof(ProductAddedToShoppingCartDomainEvent));
            Map<ProductRemovedFromShoppingCartDomainEvent>(nameof(ProductRemovedFromShoppingCartDomainEvent));
            
            // Configure integration events
            Map<OrderPlacedIntegrationEvent>(nameof(OrderPlacedIntegrationEvent));
        }
    }
}