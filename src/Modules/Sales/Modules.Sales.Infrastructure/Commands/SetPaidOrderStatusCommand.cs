using System;

using VShop.SharedKernel.Infrastructure.Commands.Contracts;

namespace VShop.Modules.Sales.Infrastructure.Commands
{
    public partial class SetPaidOrderStatusCommand : ICommand
    {
        public SetPaidOrderStatusCommand(Guid orderId)
        {
            OrderId = orderId;
        }
    }
}