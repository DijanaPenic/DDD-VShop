using System;
using System.Collections.Generic;

using VShop.SharedKernel.Messaging.Events;

namespace VShop.Modules.Catalog.Integration.Events
{
    // Notification for Sales - need to finalize the order and start the shipping process.
    public record OrderStockProcessedIntegrationEvent : IntegrationEvent 
    {
        public Guid OrderId { get; }
        public IList<OrderLine> OrderLines { get; }

        public OrderStockProcessedIntegrationEvent
        (
            Guid orderId,
            IList<OrderLine> orderLines,
            Guid causationId = default,
            Guid correlationId = default
        )
        {
            OrderId = orderId;
            OrderLines = orderLines;
            CausationId = causationId;
            CorrelationId = correlationId;
        }
        
        public record OrderLine
        {
            public Guid ProductId { get; }
            public int RequestedQuantity { get; }
            public int OutOfStockQuantity { get; }
            
            public OrderLine(Guid productId, int requestedQuantity, int outOfStockQuantity)
            {
                ProductId = productId;
                RequestedQuantity = requestedQuantity;
                OutOfStockQuantity = outOfStockQuantity;
            }
        }
    }
}