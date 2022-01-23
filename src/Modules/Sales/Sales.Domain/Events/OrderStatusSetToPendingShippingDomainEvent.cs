using System;

using VShop.SharedKernel.Infrastructure.Events;
using VShop.SharedKernel.Infrastructure.Messaging;

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