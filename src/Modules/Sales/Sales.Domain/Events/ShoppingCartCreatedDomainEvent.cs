using System;

using VShop.SharedKernel.Domain;

namespace VShop.Modules.Sales.Domain.Events
{
    public record ShoppingCartCreatedDomainEvent : IDomainEvent
    {
        public Guid ShoppingCartId { get; init; }
        public Guid CustomerId { get; init; }
        public int CustomerDiscount { get; init; }
    }
}