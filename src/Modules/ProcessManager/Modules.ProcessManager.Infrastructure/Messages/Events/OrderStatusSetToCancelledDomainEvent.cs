using VShop.SharedKernel.Infrastructure.Events.Contracts;

namespace VShop.Modules.ProcessManager.Infrastructure.Messages.Events;

internal partial class OrderStatusSetToCancelledDomainEvent : IDomainEvent
{
    public OrderStatusSetToCancelledDomainEvent(Guid orderId)
    {
        OrderId = orderId;
    }
}