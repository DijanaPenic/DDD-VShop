using System;

using VShop.SharedKernel.Infrastructure.Messaging;
using VShop.SharedKernel.Infrastructure.Events.Contracts;

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