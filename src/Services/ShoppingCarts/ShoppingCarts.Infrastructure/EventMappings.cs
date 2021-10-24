using VShop.Services.ShoppingCarts.Domain.Events;

using static VShop.SharedKernel.EventSourcing.EventTypeMapper;

namespace VShop.Services.ShoppingCarts.Infrastructure
{
    public static class EventMappings
    {
        public static void MapEventTypes()
        {
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
        }
    }
}