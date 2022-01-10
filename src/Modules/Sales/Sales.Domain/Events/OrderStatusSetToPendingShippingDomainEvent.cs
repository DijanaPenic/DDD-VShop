using System;

using VShop.SharedKernel.Messaging.Events;

namespace VShop.Modules.Sales.Domain.Events
{
    public record OrderStatusSetToPendingShippingDomainEvent //: DomainEvent
    {
        public Guid OrderId { get; }
        
        public OrderStatusSetToPendingShippingDomainEvent(Guid orderId)
        {
            OrderId = orderId;
        }
    }
}