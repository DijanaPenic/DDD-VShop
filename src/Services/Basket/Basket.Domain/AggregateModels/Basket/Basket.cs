using VShop.Services.Basket.Domain.Events;
using VShop.SharedKernel.Infrastructure.Domain;

namespace VShop.Services.Basket.Domain.AggregateModels.Basket
{
    public static class Basket
    {
        public static BasketState.Result Create(EntityId customerId)
            => new BasketState().Apply
            (
                new BasketCreatedDomainEvent
                {
                    CustomerId = customerId
                }
            );
    }
}