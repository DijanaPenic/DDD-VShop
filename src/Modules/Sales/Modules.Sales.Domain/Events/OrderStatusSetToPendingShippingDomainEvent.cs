using System;

using VShop.SharedKernel.Infrastructure.Messaging;
using VShop.SharedKernel.Infrastructure.Events.Contracts;

namespace VShop.Modules.Sales.Domain.Events
{
    public partial class OrderStatusSetToPendingShippingDomainEvent : IDomainEvent
    {
        public OrderStatusSetToPendingShippingDomainEvent(Guid orderId)
        {
            OrderId = orderId;
        }
    }
}