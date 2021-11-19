using System;

using VShop.SharedKernel.Messaging.Events;

namespace VShop.Modules.Sales.Domain.Events
{
    public record ShoppingCartProductRemovedDomainEvent : DomainEvent
    {
        public Guid ShoppingCartId { get; init; }
        public Guid ProductId { get; init; }
    }
}