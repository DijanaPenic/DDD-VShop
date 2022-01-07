using System;
using System.Collections.Generic;

using VShop.SharedKernel.Messaging.Events;

namespace VShop.Modules.Sales.Integration.Events
{
    // Notification for Catalog - need to perform the "out of stock" check.
    public record OrderStatusSetToPaidIntegrationEvent : IntegrationEvent
    {
        public Guid OrderId { get; }
        public IList<OrderLine> OrderLines { get; }
        
        public OrderStatusSetToPaidIntegrationEvent(Guid orderId, IList<OrderLine> orderLines)
        {
            OrderId = orderId;
            OrderLines = orderLines;
        }
        
        public record OrderLine
        {
            public Guid ProductId { get; }
            public int Quantity { get; }
            public decimal Price { get; }
            
            public OrderLine(Guid productId, int quantity, decimal price)
            {
                ProductId = productId;
                Quantity = quantity;
                Price = price;
            }
        }
    }
}