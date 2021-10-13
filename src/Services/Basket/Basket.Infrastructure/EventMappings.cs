using VShop.Services.Basket.Domain.Events;

using static VShop.SharedKernel.EventSourcing.TypeMapper;

namespace VShop.Services.Basket.Infrastructure
{
    public static class EventMappings
    {
        public static void MapEventTypes()
        {
            // TODO - check if I can omit this code
            Map<BasketCreatedDomainEvent>("BasketCreated");
            Map<BasketCheckoutRequestedDomainEvent>("BasketCheckoutRequested");
            Map<BasketDeletionRequestedDomainEvent>("BasketDeletionRequested");
            Map<BasketItemQuantityDecreasedDomainEvent>("BasketItemQuantityDecreased");
            Map<BasketItemQuantityIncreasedDomainEvent>("BasketItemQuantityIncreased");
            Map<ContactInformationSetDomainEvent>("ContactInformationSet");
            Map<DeliveryAddressSetDomainEvent>("DeliveryAddressSet");
            Map<DeliveryCostChangedDomainEvent>("DeliveryCostChanged");
            Map<ProductAddedToBasketDomainEvent>("ProductAddedToBasket");
            Map<ProductRemovedFromBasketDomainEvent>("ProductRemovedFromBasket");
        }
    }
}