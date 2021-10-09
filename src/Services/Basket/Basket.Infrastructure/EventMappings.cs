using VShop.Services.Basket.Domain.Events;

using static VShop.SharedKernel.EventSourcing.TypeMapper;

namespace VShop.Services.Basket.Infrastructure
{
    public static class EventMappings
    {
        public static void MapEventTypes()
        {
            // TODO - check if I can reference domain project
            Map<BasketCreatedDomainEvent>("BasketCreated");
        }
    }
}