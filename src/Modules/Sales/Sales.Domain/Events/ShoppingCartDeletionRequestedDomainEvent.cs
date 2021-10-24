using System;

using VShop.SharedKernel.Domain;

namespace VShop.Modules.Sales.Domain.Events
{
    public record ShoppingCartDeletionRequestedDomainEvent : IDomainEvent
    {
        public Guid ShoppingCartId { get; init; }
    }
}