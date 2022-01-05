using System;

using VShop.SharedKernel.Messaging.Events;

namespace VShop.Modules.Sales.Domain.Events
{
    public record OrderStatusSetToShippedDomainEvent : DomainEvent
    {
        public Guid OrderId { get; }
        
        public OrderStatusSetToShippedDomainEvent(Guid orderId)
        {
            OrderId = orderId;
        }
    }
}