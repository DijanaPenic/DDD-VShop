using NodaTime;
using NodaTime.Serialization.Protobuf;

using VShop.SharedKernel.Infrastructure.Events.Contracts;

namespace VShop.Modules.ProcessManager.Infrastructure.Messages.Events;

internal partial class ShoppingCartCheckoutRequestedDomainEvent : IDomainEvent
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