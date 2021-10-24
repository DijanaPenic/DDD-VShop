using System;

using VShop.SharedKernel.Domain;

namespace VShop.Modules.Sales.Domain.Events
{
    public record DeliveryCostChangedDomainEvent : IDomainEvent
    {
        public Guid ShoppingCartId { get; init; }
        public decimal DeliveryCost { get; init; }
    }
}