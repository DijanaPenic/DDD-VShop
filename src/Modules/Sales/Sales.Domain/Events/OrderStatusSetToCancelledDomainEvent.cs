using System;

using VShop.SharedKernel.Messaging;
using VShop.SharedKernel.Messaging.Events;

namespace VShop.Modules.Sales.Domain.Events
{
    public partial class OrderStatusSetToCancelledDomainEvent : MessageContext, IDomainEvent
    {
        public OrderStatusSetToCancelledDomainEvent(Guid orderId)
        {
            OrderId = orderId;
        }
    }
}