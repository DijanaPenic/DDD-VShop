using System;
using NodaTime;
using NodaTime.Serialization.Protobuf;

using VShop.SharedKernel.Infrastructure.Messaging;
using VShop.SharedKernel.Infrastructure.Events.Contracts;

namespace VShop.Modules.Sales.Domain.Events
{
    public partial class ShoppingCartCheckoutRequestedDomainEvent : IDomainEvent
    {
        public ShoppingCartCheckoutRequestedDomainEvent
        (
            Guid shoppingCartId,
            Guid orderId,
            Instant confirmedAt
        )
        {
            ShoppingCartId = shoppingCartId;
            OrderId = orderId;
            ConfirmedAt = confirmedAt.ToTimestamp();
        }
    }
}