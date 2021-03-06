using VShop.SharedKernel.Infrastructure.Events.Contracts;

namespace VShop.Modules.ProcessManager.Infrastructure.Messages.Events;

internal partial class OrderStatusSetToShippedDomainEvent : IDomainEvent
{
    public OrderStatusSetToShippedDomainEvent(Guid orderId)
    {
        OrderId = orderId;
    }
}