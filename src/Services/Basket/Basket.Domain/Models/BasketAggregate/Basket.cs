using System;
using System.Collections.Generic;

using VShop.Services.Basket.Domain.Events;
using VShop.SharedKernel.EventSourcing;
using VShop.SharedKernel.Infrastructure.Domain;

namespace VShop.Services.Basket.Domain.Models.BasketAggregate
{
    public class Basket : AggregateRoot
    {
        public EntityId CustomerId { get; private set; }

        public BasketStatus Status { get; private set; }
        
        private readonly List<BasketItem> _basketItems;
        public IReadOnlyCollection<BasketItem> BasketItems => _basketItems;
        
        public enum BasketStatus { New, PendingCheckout, Closed }

        public static Basket Create(EntityId customerId)
        {
            Basket basket = new Basket();
            
            basket.Apply
            (
                new BasketCreatedDomainEvent
                {
                    Id = Guid.NewGuid(), // TODO - sequential guid
                    CustomerId = customerId
                }
            );

            return basket;
        }

        protected override void When(object @event)
        {
            switch (@event)
            {
                case BasketCreatedDomainEvent e:
                    Id = new EntityId(e.Id);
                    CustomerId = new EntityId(e.CustomerId);
                    Status = BasketStatus.New;
                    break;
            }
        }
    }
}