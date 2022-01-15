using System;
using System.Collections.Generic;

using VShop.SharedKernel.Messaging;
using VShop.SharedKernel.Messaging.Events;
using VShop.SharedKernel.Infrastructure.Extensions;

namespace VShop.Modules.Sales.Integration.Events
{
    // Notification for Billing - need to perform partial or full refund.
    public partial class OrderFinalizedIntegrationEvent : MessageContext, IIntegrationEvent
    {
        public OrderFinalizedIntegrationEvent(Guid orderId, decimal refundAmount, IEnumerable<Types.OrderLine> orderLines)
        {
            OrderId = orderId;
            RefundAmount = refundAmount.ToMoney();
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