using System;

using VShop.SharedKernel.Application.Events;

namespace VShop.Modules.Sales.Integration.Events
{
    public record OrderPlacedIntegrationEvent : IIntegrationEvent
    {
        public Guid OrderId { get; init; }
    }
}