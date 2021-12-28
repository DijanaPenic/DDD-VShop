using System;
using System.Collections.Generic;

using VShop.SharedKernel.Messaging.Events;

namespace VShop.Modules.Sales.Integration.Events
{
    // Notification for Shipping - need to ship the order.
    public record OrderStatusSetToPendingShippingIntegrationEvent : IntegrationEvent
    {
        public Guid OrderId { get; init; }
        public IList<OrderLine> OrderLines { get; init; }
        
        public record OrderLine
        {
            public Guid ProductId { get; init; }
            public int Quantity { get; set; }
        }
    }
}