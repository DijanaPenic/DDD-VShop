using VShop.SharedKernel.Infrastructure.Events.Contracts;

namespace VShop.Modules.Catalog.Integration.Events
{
    // Notification for Sales - need to finalize the order and start the shipping process.
    public partial class OrderStockProcessedIntegrationEvent : IIntegrationEvent 
    {
        public OrderStockProcessedIntegrationEvent
        (
            Guid orderId,
            IEnumerable<Types.OrderLine> orderLines
        )
        {
            OrderId = orderId;
            OrderLines.AddRange(orderLines);
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