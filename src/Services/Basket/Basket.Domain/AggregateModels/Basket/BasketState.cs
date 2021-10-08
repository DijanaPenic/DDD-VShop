using VShop.SharedKernel.EventSourcing;
using VShop.Services.Basket.Domain.Events;

namespace VShop.Services.Basket.Domain.AggregateModels.Basket
{
    public class BasketState : AggregateState<BasketState>
    {
        public override BasketState When(BasketState state, object @event)
            => With(
                @event switch
                {
                    BasketCreatedDomainEvent e =>
                        With(state, s => s.CustomerId = new CustomerId(e.CustomerId)),
                    _ => this
                }, 
                s => s.Version++
            );
        
        protected override bool EnsureValidState(BasketState newState)
            => true;

        public CustomerId CustomerId { get; private set; }

        public BasketStatus Status { get; }
        
        public enum BasketStatus { New, PendingCheckout, Closed }
    }
}