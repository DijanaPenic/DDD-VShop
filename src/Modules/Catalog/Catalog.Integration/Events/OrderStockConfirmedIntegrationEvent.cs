using System;
using System.Collections.Generic;

using VShop.SharedKernel.Messaging.Events;

namespace VShop.Modules.Catalog.Integration.Events
{
    // Notification for Sales - need to finalize the order and start the shipping process.
    // Notification for Billing - need to refund out of stock items.
    public record OrderStockConfirmedIntegrationEvent : IntegrationEvent 
    {
        public Guid OrderId { get; init; }
        public IList<OrderLine> OrderLines { get; init; }
        
        public record OrderLine
        {
            public Guid ProductId { get; init; }
            public int RequestedQuantity { get; init; }
            public int OutOfStockQuantity { get; init; }
            public bool EnoughStock => OutOfStockQuantity is 0;
        }
    }
}