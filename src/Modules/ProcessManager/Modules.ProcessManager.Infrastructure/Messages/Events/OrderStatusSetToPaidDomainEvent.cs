using VShop.SharedKernel.Infrastructure.Events.Contracts;

namespace VShop.Modules.ProcessManager.Infrastructure.Messages.Events;

internal partial class OrderStatusSetToPaidDomainEvent : IDomainEvent
{
    public OrderStatusSetToPaidDomainEvent(Guid orderId)
    {
        OrderId = orderId;
    }
}