using System;

using VShop.SharedKernel.Infrastructure.Commands.Contracts;

namespace VShop.Modules.Sales.Infrastructure.Commands
{
    public partial class CancelOrderCommand : ICommand
    {
        public CancelOrderCommand(Guid orderId)
        {
            OrderId = orderId;
        }
    }
}