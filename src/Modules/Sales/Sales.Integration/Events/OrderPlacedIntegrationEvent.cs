using System;

using VShop.SharedKernel.Infrastructure.Messaging;

namespace VShop.Modules.Sales.Integration.Events
{
    public record OrderPlacedIntegrationEvent : BaseIntegrationEvent
    {
        public Guid OrderId { get; init; }
    }
}