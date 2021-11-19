using System;

using VShop.SharedKernel.Messaging.Events;

namespace VShop.Modules.Sales.Domain.Events
{
    public record ShoppingCartProductAddedDomainEvent : DomainEvent
    {
        public Guid ShoppingCartId { get; init; }
        public Guid ProductId { get; init; }
        public int Quantity { get; init; }
        public decimal UnitPrice { get; init; }
    }
}