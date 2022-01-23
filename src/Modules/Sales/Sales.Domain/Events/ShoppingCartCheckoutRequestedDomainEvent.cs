using System;
using NodaTime;
using NodaTime.Serialization.Protobuf;

using VShop.SharedKernel.Infrastructure.Events;
using VShop.SharedKernel.Infrastructure.Messaging;

namespace VShop.Modules.Sales.Domain.Events
{
    public partial class ShoppingCartCheckoutRequestedDomainEvent : MessageContext, IDomainEvent
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