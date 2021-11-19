using System;

using VShop.SharedKernel.Messaging.Events;

namespace VShop.Modules.Sales.Domain.Events
{
    public record ShoppingCartDeliveryCostChangedDomainEvent : DomainEvent
    {
        public Guid ShoppingCartId { get; init; }
        public decimal DeliveryCost { get; init; }
    }
}