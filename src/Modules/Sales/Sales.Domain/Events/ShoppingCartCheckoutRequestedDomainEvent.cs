using System;

using VShop.SharedKernel.Infrastructure.Messaging;

namespace VShop.Modules.Sales.Domain.Events
{
    public record ShoppingCartCheckoutRequestedDomainEvent : BaseDomainEvent
    {
        public Guid ShoppingCartId { get; init; }
        public DateTime ConfirmedAt { get; init; }
    }
}