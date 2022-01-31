using VShop.SharedKernel.Infrastructure.Commands.Contracts;

namespace VShop.Modules.ProcessManager.Infrastructure.Messages.Commands;

internal partial class DeleteShoppingCartCommand : ICommand
{
    public DeleteShoppingCartCommand(Guid shoppingCartId)
    {
        ShoppingCartId = shoppingCartId;
    }
}