using VShop.SharedKernel.Infrastructure.Events.Contracts;

namespace VShop.Modules.ProcessManager.Infrastructure.Messages.Events;

internal partial class OrderStatusSetToPendingShippingDomainEvent : IDomainEvent
{
    public OrderStatusSetToPendingShippingDomainEvent(Guid orderId)
    {
        OrderId = orderId;
    }
}