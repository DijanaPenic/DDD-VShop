using System;

using VShop.SharedKernel.Messaging.Events;

namespace VShop.Modules.Sales.Domain.Events
{
    public record ShippingGracePeriodExpiredDomainEvent : DomainEvent
    {
        public Guid OrderId { get; }
        public string Content { get; }
        
        public ShippingGracePeriodExpiredDomainEvent(Guid orderId, string content)
        {
            OrderId = orderId;
            Content = content;
        }
    }
}