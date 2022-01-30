using System;

using VShop.SharedKernel.Infrastructure.Commands.Contracts;

namespace VShop.Modules.Sales.Infrastructure.Commands
{
    internal partial class CancelOrderCommand : ICommand
    {
        public CancelOrderCommand(Guid orderId)
        {
            OrderId = orderId;
        }
    }
}