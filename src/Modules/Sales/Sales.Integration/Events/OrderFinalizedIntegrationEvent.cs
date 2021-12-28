using System;
using System.Collections.Generic;

using VShop.SharedKernel.Messaging.Events;

namespace VShop.Modules.Sales.Integration.Events
{
    // Notification for Billing - need to perform partial or full refund.
    public record OrderFinalizedIntegrationEvent : IntegrationEvent
    {
        public Guid OrderId { get; init; }
        public decimal RefundAmount { get; init; }
        
        public IList<OrderLine> OrderLines { get; init; }
        
        public record OrderLine
        {
            public Guid ProductId { get; init; }
            public int Quantity { get; set; }
        }
    }
}