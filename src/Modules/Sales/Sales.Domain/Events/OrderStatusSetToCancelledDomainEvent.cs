using System;

using VShop.SharedKernel.Messaging.Events;

namespace VShop.Modules.Sales.Domain.Events
{
    public record OrderStatusSetToCancelledDomainEvent : DomainEvent
    {
        public Guid OrderId { get; init; }
    }
}