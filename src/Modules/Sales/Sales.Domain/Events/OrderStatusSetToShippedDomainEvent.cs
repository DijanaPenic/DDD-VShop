using System;

using VShop.SharedKernel.Messaging;
using VShop.SharedKernel.Messaging.Events;

namespace VShop.Modules.Sales.Domain.Events
{
    public partial class OrderStatusSetToShippedDomainEvent : MessageContext, IDomainEvent
    {
        public OrderStatusSetToShippedDomainEvent(Guid orderId)
        {
            OrderId = orderId;
        }
    }
}