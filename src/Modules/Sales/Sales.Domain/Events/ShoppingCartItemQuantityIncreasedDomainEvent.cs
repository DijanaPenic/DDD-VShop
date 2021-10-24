using System;

using VShop.SharedKernel.Domain;

namespace VShop.Modules.Sales.Domain.Events
{
    public record ShoppingCartItemQuantityIncreasedDomainEvent : IDomainEvent
    {
        public Guid ShoppingCartId { get; init; }
        public Guid ProductId { get; init; }
        public int Quantity { get; init; }
    }
}