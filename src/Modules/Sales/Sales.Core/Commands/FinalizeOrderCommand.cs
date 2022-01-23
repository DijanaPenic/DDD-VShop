using System;
using System.Collections.Generic;

using VShop.SharedKernel.Infrastructure.Commands;
using VShop.SharedKernel.Infrastructure.Messaging;

namespace VShop.Modules.Sales.Core.Commands
{
    public partial class FinalizeOrderCommand : MessageContext, ICommand
    {
        public FinalizeOrderCommand
        (
            Guid orderId,
            IEnumerable<Types.OrderLine> orderLines,
            MessageMetadata metadata
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