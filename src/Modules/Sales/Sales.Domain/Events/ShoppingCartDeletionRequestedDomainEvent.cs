using System;

using VShop.SharedKernel.Infrastructure.Messaging.Events;

namespace VShop.Modules.Sales.Domain.Events
{
    public record ShoppingCartDeletionRequestedDomainEvent : BaseDomainEvent
    {
        public Guid ShoppingCartId { get; init; }
    }
}