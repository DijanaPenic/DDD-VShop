using System;

using VShop.SharedKernel.Infrastructure.Messaging;

namespace VShop.Modules.Sales.Domain.Events
{
    public record ShoppingCartDeliveryCostChangedDomainEvent : IDomainEvent
    {
        public Guid ShoppingCartId { get; init; }
        public decimal DeliveryCost { get; init; }
    }
}