using VShop.SharedKernel.EventSourcing;

namespace VShop.Services.Basket.Domain.Models.BasketAggregate
{
    public class BasketItemState : AggregateState<BasketItemState>
    {
        public override BasketItemState When(BasketItemState state, object @event)
            => With(
                @event switch
                {
                    _ => this
                }, 
                s => s.Version++
            );
        
        protected override bool EnsureValidState(BasketItemState newState)
            => true;
    }
}