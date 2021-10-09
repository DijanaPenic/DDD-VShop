﻿using System.Collections.Generic;

using VShop.SharedKernel.EventSourcing;
using VShop.SharedKernel.Infrastructure.Domain;
using VShop.Services.Basket.Domain.Events;

namespace VShop.Services.Basket.Domain.Models.BasketAggregate
{
    public class BasketState : AggregateState<BasketState>
    {
        public override BasketState When(BasketState state, object @event)
            => With(
                @event switch
                {
                    BasketCreatedDomainEvent e =>
                        With(state, s =>
                        {
                            s.Id = e.Id;
                            s.CustomerId = new EntityId(e.CustomerId);
                            s.Status = BasketStatus.New;
                        }),
                    _ => this
                }, 
                s => s.Version++
            );
        
        protected override bool EnsureValidState(BasketState newState)
            => true;

        public EntityId CustomerId { get; private set; }

        public BasketStatus Status { get; private set; }
        
        public IList<BasketItemState>  Items { get; private set; }
        
        public enum BasketStatus { New, PendingCheckout, Closed }
    }
}