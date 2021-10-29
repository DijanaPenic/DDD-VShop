using System;

using VShop.SharedKernel.Infrastructure.Messaging;

namespace VShop.Modules.Sales.Integration.Events
{
    public record OrderPlacedIntegrationEvent : IIntegrationEvent
    {
        public Guid OrderId { get; init; }
    }
}