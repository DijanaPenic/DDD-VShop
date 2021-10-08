using VShop.Services.Basket.Domain.Events;

namespace VShop.Services.Basket.Domain.AggregateModels.Basket
{
    public static class Basket
    {
        public static BasketState.Result Create(CustomerId customerId)
            => new BasketState().Apply
            (
                new BasketCreatedDomainEvent
                {
                    CustomerId = customerId
                }
            );
    }
}