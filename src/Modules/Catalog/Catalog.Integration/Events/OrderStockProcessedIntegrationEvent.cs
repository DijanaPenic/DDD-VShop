using System;
using System.Collections.Generic;

using VShop.SharedKernel.Messaging;
using VShop.SharedKernel.Messaging.Events;

namespace VShop.Modules.Catalog.Integration.Events
{
    // Notification for Sales - need to finalize the order and start the shipping process.
    public partial class OrderStockProcessedIntegrationEvent : MessageContext, IIntegrationEvent 
    {
        public OrderStockProcessedIntegrationEvent
        (
            Guid orderId,
            IEnumerable<Types.OrderLine> orderLines,
            MessageMetadata metadata
        )
        {
            OrderId = orderId;
            OrderLines.AddRange(orderLines);
            Metadata = metadata;
        }

        public partial class Types
        {
            public partial class OrderLine
            {
                public OrderLine(Guid productId, int requestedQuantity, int outOfStockQuantity)
                {
                    ProductId = productId;
                    RequestedQuantity = requestedQuantity;
                    OutOfStockQuantity = outOfStockQuantity;
                }
            }
        }
    }
}