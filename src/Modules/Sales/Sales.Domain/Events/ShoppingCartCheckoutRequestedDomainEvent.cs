using System;

using VShop.SharedKernel.Messaging.Events;

namespace VShop.Modules.Sales.Domain.Events
{
    public record ShoppingCartCheckoutRequestedDomainEvent : DomainEvent
    {
        public Guid ShoppingCartId { get; init; }
        public Guid OrderId { get; init; }
        public DateTime ConfirmedAt { get; init; }
    }
}