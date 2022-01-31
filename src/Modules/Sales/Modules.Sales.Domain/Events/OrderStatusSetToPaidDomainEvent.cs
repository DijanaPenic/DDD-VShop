using System;

using VShop.SharedKernel.Infrastructure.Events.Contracts;

namespace VShop.Modules.Sales.Domain.Events
{
    internal partial class OrderStatusSetToPaidDomainEvent : IDomainEvent
    {
        public OrderStatusSetToPaidDomainEvent(Guid orderId)
        {
            OrderId = orderId;
        }
    }
}