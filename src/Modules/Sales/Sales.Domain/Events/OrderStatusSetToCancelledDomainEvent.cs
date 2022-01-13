using System;

using VShop.SharedKernel.Messaging.Events;

namespace VShop.Modules.Sales.Domain.Events
{
    public partial class OrderStatusSetToCancelledDomainEvent : IDomainEvent
    {
        public OrderStatusSetToCancelledDomainEvent(Guid orderId)
        {
            OrderId = orderId;
        }
    }
}