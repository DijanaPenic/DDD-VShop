using System;
using System.Collections.Generic;

using VShop.SharedKernel.Messaging.Events;

namespace VShop.Modules.Sales.Integration.Events
{
    // Notification for Billing - need to perform partial or full refund.
    public record OrderFinalizedIntegrationEvent //: IntegrationEvent
    {
        public Guid OrderId { get; }
        public decimal RefundAmount { get; }
        public IList<OrderLine> OrderLines { get; }

        public OrderFinalizedIntegrationEvent(Guid orderId, decimal refundAmount, IList<OrderLine> orderLines)
        {
            OrderId = orderId;
            RefundAmount = refundAmount;
            OrderLines = orderLines;
        }

        public record OrderLine
        {
            public Guid ProductId { get; }
            public int Quantity { get; }
            
            public OrderLine(Guid productId, int quantity)
            {
                ProductId = productId;
                Quantity = quantity;
            }
        }
    }
}