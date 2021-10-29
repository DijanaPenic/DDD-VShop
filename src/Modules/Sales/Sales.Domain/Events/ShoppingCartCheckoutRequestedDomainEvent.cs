using System;

using VShop.SharedKernel.Infrastructure.Messaging;

namespace VShop.Modules.Sales.Domain.Events
{
    public record ShoppingCartCheckoutRequestedDomainEvent : IDomainEvent
    {
        public Guid ShoppingCartId { get; init; }
        public DateTime ConfirmedAt { get; init; }
    }
}