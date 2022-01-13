using System;
using System.Collections.Generic;

using VShop.SharedKernel.Messaging.Commands;

namespace VShop.Modules.Sales.API.Application.Commands
{
    public partial class FinalizeOrderCommand : IBaseCommand
    {
        public FinalizeOrderCommand
        (
            Guid orderId,
            IEnumerable<Types.OrderLine> orderLines
        )
        {
            OrderId = orderId;
            OrderLines.Add(orderLines);
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