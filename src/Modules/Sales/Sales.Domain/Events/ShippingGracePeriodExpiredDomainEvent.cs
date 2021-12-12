using System;
using Newtonsoft.Json;

using VShop.SharedKernel.Messaging.Events;

namespace VShop.Modules.Sales.Domain.Events
{
    public record ShippingGracePeriodExpiredDomainEvent : DomainEvent
    {
        public Guid OrderId { get; }
        public string Content { get; }
        
        public ShippingGracePeriodExpiredDomainEvent(Guid orderId, IBaseEvent @event)
        {
            OrderId = orderId;
            Content = JsonConvert.SerializeObject(@event);
        }
    }
}