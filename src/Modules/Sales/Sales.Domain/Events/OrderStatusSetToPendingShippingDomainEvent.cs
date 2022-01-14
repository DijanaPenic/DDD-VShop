using System;

using VShop.SharedKernel.Messaging;
using VShop.SharedKernel.Messaging.Events;

namespace VShop.Modules.Sales.Domain.Events
{
    public partial class OrderStatusSetToPendingShippingDomainEvent : MessageContext, IDomainEvent
    {
        public OrderStatusSetToPendingShippingDomainEvent(Guid orderId)
        {
            OrderId = orderId;
        }
    }
}