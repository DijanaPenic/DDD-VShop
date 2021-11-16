using System;

using VShop.SharedKernel.Infrastructure.Messaging.Events;

namespace VShop.Modules.Sales.Domain.Events
{
    public record ShoppingCartCreatedDomainEvent : DomainEvent
    {
        public Guid ShoppingCartId { get; init; }
        public Guid CustomerId { get; init; }
        public int CustomerDiscount { get; init; }
    }
}