using VShop.Services.ShoppingCarts.Domain.Events;

using static VShop.SharedKernel.EventSourcing.EventTypeMapper;

namespace VShop.Services.ShoppingCarts.Infrastructure
{
    public static class EventMappings
    {
        public static void MapEventTypes()
        {
            Map<ShoppingCartCreatedDomainEvent>("ShoppingCartCreated");
            Map<ShoppingCartCheckoutRequestedDomainEvent>("ShoppingCartCheckoutRequested");
            Map<ShoppingCartDeletionRequestedDomainEvent>("ShoppingCartDeletionRequested");
            Map<ShoppingCartItemQuantityDecreasedDomainEvent>("ShoppingCartItemQuantityDecreased");
            Map<ShoppingCartItemQuantityIncreasedDomainEvent>("ShoppingCartItemQuantityIncreased");
            Map<ContactInformationSetDomainEvent>("ContactInformationSet");
            Map<DeliveryAddressSetDomainEvent>("DeliveryAddressSet");
            Map<DeliveryCostChangedDomainEvent>("DeliveryCostChanged");
            Map<ProductAddedToShoppingCartDomainEvent>("ProductAddedToShoppingCart");
            Map<ProductRemovedFromShoppingCartDomainEvent>("ProductRemovedFromShoppingCart");
        }
    }
}