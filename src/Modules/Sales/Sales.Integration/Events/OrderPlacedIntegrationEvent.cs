using System;
using System.Collections.Generic;

using VShop.SharedKernel.Messaging.Events;

namespace VShop.Modules.Sales.Integration.Events
{
    public record OrderPlacedIntegrationEvent : IntegrationEvent
    {
        public Guid OrderId { get; init; }
        public IList<PlacedOrderLine> OrderLines { get; init; }
    }
    
    public record PlacedOrderLine
    {
        public Guid ProductId { get; init; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}