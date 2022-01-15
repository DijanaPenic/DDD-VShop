using System;
using System.Collections.Generic;

using VShop.SharedKernel.Messaging;
using VShop.SharedKernel.Messaging.Commands;

namespace VShop.Modules.Sales.API.Application.Commands
{
    public partial class FinalizeOrderCommand : Command, IBaseCommand
    {
        public FinalizeOrderCommand
        (
            Guid orderId,
            IEnumerable<Types.OrderLine> orderLines,
            MessageMetadata metadata = default
        )
        {
            OrderId = orderId;
            OrderLines.Add(orderLines);
            Metadata = metadata;
        }
        
        public partial class Types
        {
            public partial class OrderLine
            {
                public OrderLine(Guid productId, int outOfStockQuantity)
                {
                    ProductId = productId;
                    OutOfStockQuantity = outOfStockQuantity;
                }
            }
        }
    }
}