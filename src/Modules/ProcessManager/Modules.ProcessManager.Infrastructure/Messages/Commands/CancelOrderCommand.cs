using VShop.SharedKernel.Infrastructure.Commands.Contracts;

namespace VShop.Modules.ProcessManager.Infrastructure.Messages.Commands;

internal partial class CancelOrderCommand : ICommand
{
    public CancelOrderCommand(Guid orderId)
    {
        OrderId = orderId;
    }
}