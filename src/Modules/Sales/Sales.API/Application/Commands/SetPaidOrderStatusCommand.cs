using System;

namespace VShop.Modules.Sales.API.Application.Commands
{
    public partial class SetPaidOrderStatusCommand
    {
        public SetPaidOrderStatusCommand(Guid orderId)
        {
            OrderId = orderId;
        }
    }
}