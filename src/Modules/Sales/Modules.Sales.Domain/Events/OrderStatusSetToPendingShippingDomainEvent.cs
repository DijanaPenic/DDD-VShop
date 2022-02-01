using System;

using VShop.SharedKernel.Infrastructure.Events.Contracts;

namespace VShop.Modules.Sales.Domain.Events
{
    internal partial class OrderStatusSetToPendingShippingDomainEvent : IDomainEvent
    {
        public OrderStatusSetToPendingShippingDomainEvent(Guid orderId)
        {
            OrderId = orderId;
        }
    }
}