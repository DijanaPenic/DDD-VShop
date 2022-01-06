using System;

using VShop.SharedKernel.Messaging.Events;

namespace VShop.Modules.Sales.Domain.Events
{
    public record ShoppingCartDeliveryCostChangedDomainEvent : DomainEvent
    {
        public Guid ShoppingCartId { get; }
        public decimal DeliveryCost { get; }

        public ShoppingCartDeliveryCostChangedDomainEvent
        (
            Guid shoppingCartId,
            decimal deliveryCost
        )
        {
            ShoppingCartId = shoppingCartId;
            DeliveryCost = deliveryCost;
        }
    }
}