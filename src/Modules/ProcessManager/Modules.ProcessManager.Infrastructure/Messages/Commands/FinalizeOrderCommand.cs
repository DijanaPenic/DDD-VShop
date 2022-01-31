using VShop.SharedKernel.Infrastructure.Commands.Contracts;

namespace VShop.Modules.ProcessManager.Infrastructure.Messages.Commands;

internal partial class FinalizeOrderCommand : ICommand
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
        internal partial class OrderLine
        {
            public OrderLine(Guid productId, int outOfStockQuantity)
            {
                ProductId = productId;
                OutOfStockQuantity = outOfStockQuantity;
            }
        }
    }
}