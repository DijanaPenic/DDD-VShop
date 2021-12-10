using System;
using NodaTime;

using VShop.SharedKernel.Messaging.Events;

namespace VShop.Modules.Sales.Domain.Events
{
    public record ShoppingCartCheckoutRequestedDomainEvent : DomainEvent
    {
        public Guid ShoppingCartId { get; init; }
        public Guid OrderId { get; init; }
        public Instant ConfirmedAt { get; init; }
    }
}