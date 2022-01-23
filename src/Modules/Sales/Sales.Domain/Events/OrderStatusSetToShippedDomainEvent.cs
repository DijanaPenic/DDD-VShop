using System;

using VShop.SharedKernel.Infrastructure.Events;
using VShop.SharedKernel.Infrastructure.Messaging;

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