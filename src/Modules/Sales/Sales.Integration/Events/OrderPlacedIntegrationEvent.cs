using System;

using VShop.SharedKernel.Messaging.Events;

namespace VShop.Modules.Sales.Integration.Events
{
    public record OrderPlacedIntegrationEvent : IntegrationEvent
    {
        public Guid OrderId { get; init; }
    }
}