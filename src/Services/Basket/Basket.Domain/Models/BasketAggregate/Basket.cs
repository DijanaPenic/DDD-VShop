using System;

using VShop.Services.Basket.Domain.Events;
using VShop.SharedKernel.Infrastructure.Domain;

namespace VShop.Services.Basket.Domain.Models.BasketAggregate
{
    public static class Basket
    {
        public static BasketState.Result Create(EntityId customerId)
            => new BasketState().Apply
            (
                new BasketCreatedDomainEvent
                {
                    Id = Guid.NewGuid(), // TODO - sequential guid
                    CustomerId = customerId
                }
            );

        public static BasketState.Result AddItem(BasketItemState item)
        {
        

            return default;
        }
    }
}