using System;
using NodaTime;

using VShop.SharedKernel.Messaging.Events;

namespace VShop.Modules.Sales.Domain.Events
{
    public record ShoppingCartCheckoutRequestedDomainEvent// : DomainEvent
    {
        public Guid ShoppingCartId { get; }
        public Guid OrderId { get; }
        public Instant ConfirmedAt { get; }

        public ShoppingCartCheckoutRequestedDomainEvent
        (
            Guid shoppingCartId,
            Guid orderId,
            Instant confirmedAt
        )
        {
            ShoppingCartId = shoppingCartId;
            OrderId = orderId;
            ConfirmedAt = confirmedAt;
        }
    }
}