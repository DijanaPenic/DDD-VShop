using System;

using VShop.SharedKernel.Infrastructure.Messaging;

namespace VShop.Modules.Sales.Domain.Events
{
    public record ShoppingCartDeletionRequestedDomainEvent : IDomainEvent
    {
        public Guid ShoppingCartId { get; init; }
    }
}