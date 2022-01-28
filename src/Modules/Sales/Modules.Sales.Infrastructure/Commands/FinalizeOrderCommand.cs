using System;
using System.Collections.Generic;

using VShop.SharedKernel.Infrastructure.Commands.Contracts;

namespace VShop.Modules.Sales.Infrastructure.Commands
{
    public partial class FinalizeOrderCommand : ICommand
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