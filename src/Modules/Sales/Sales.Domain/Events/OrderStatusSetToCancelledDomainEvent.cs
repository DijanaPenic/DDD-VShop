using System;

using VShop.SharedKernel.Infrastructure.Events;
using VShop.SharedKernel.Infrastructure.Messaging;

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