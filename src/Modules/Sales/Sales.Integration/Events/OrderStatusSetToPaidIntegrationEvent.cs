using System;
using System.Collections.Generic;

using VShop.SharedKernel.Messaging.Events;

namespace VShop.Modules.Sales.Integration.Events
{
    // Notification for Catalog - need to perform the "out of stock" check.
    public record OrderStatusSetToPaidIntegrationEvent : IntegrationEvent
    {
        public Guid OrderId { get; init; }
        public IList<OrderLine> OrderLines { get; init; }
        
        public record OrderLine
        {
            public Guid ProductId { get; init; }
            public int Quantity { get; init; }
            public decimal Price { get; init; }
        }
    }
}