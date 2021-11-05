using System;

using VShop.SharedKernel.Infrastructure.Messaging.Events;

namespace VShop.Modules.Sales.Integration.Events
{
    public record OrderPlacedIntegrationEvent : BaseIntegrationEvent
    {
        public Guid OrderId { get; init; }
    }
}