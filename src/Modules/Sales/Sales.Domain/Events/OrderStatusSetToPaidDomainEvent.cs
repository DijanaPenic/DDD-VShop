using System;

using VShop.SharedKernel.Infrastructure.Events;
using VShop.SharedKernel.Infrastructure.Messaging;

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