using System;
using System.Collections.Generic;

using VShop.SharedKernel.Messaging.Events;

namespace VShop.Modules.Sales.Integration.Events
{
    // Notification for Billing - need to perform partial or full refund.
    public partial class OrderFinalizedIntegrationEvent : IIntegrationEvent
    {
        public OrderFinalizedIntegrationEvent(Guid orderId, decimal refundAmount, IEnumerable<Types.OrderLine> orderLines)
        {
            OrderId = orderId;
            RefundAmount = refundAmount;
            OrderLines.Add(orderLines);
        }

        public partial class Types
        {
            public partial class OrderLine
            {
                public OrderLine(Guid productId, int quantity)
                {
                    ProductId = productId;
                    Quantity = quantity;
                }
            }
        }
    }
}