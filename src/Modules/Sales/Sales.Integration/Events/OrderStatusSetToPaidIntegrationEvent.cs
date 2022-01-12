using System;
using System.Collections.Generic;

using VShop.SharedKernel.Messaging.Events;

namespace VShop.Modules.Sales.Integration.Events
{
    // Notification for Catalog - need to perform the "out of stock" check.
    public partial class OrderStatusSetToPaidIntegrationEvent : IIntegrationEvent
    {
        public OrderStatusSetToPaidIntegrationEvent(Guid orderId, IEnumerable<Types.OrderLine> orderLines)
        {
            OrderId = orderId;
            OrderLines.Add(orderLines);
        }

        public partial class Types
        {
            public partial class OrderLine
            {
                public OrderLine(Guid productId, int quantity, decimal price)
                {
                    ProductId = productId;
                    Quantity = quantity;
                    Price = price;
                }
            }
        }
    }
}