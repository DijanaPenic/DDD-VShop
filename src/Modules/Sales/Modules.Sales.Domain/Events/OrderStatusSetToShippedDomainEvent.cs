using System;

using VShop.SharedKernel.Infrastructure.Events.Contracts;

namespace VShop.Modules.Sales.Domain.Events
{
    internal partial class OrderStatusSetToShippedDomainEvent : IDomainEvent
    {
        public OrderStatusSetToShippedDomainEvent(Guid orderId)
        {
            OrderId = orderId;
        }
    }
}