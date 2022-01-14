using System;

using VShop.SharedKernel.Messaging;
using VShop.SharedKernel.Messaging.Events;

namespace VShop.Modules.Sales.Domain.Events
{
    public partial class OrderStatusSetToPaidDomainEvent : MessageContext, IDomainEvent
    {
        public OrderStatusSetToPaidDomainEvent(Guid orderId)
        {
            OrderId = orderId;
        }
    }
}