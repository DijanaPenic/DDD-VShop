using VShop.SharedKernel.Infrastructure.Commands.Contracts;

namespace VShop.Modules.ProcessManager.Infrastructure.Messages.Commands;

internal partial class PlaceOrderCommand : ICommand
{
    public PlaceOrderCommand
    (
        Guid orderId,
        Guid shoppingCartId
    )
    {
        OrderId = orderId;
        ShoppingCartId = shoppingCartId;
    }
}