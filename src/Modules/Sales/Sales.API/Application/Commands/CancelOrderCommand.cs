using System;

using VShop.SharedKernel.Messaging.Commands;

namespace VShop.Modules.Sales.API.Application.Commands
{
    public partial class CancelOrderCommand : Command, IBaseCommand
    {
        public CancelOrderCommand(Guid orderId)
        {
            OrderId = orderId;
        }
    }
}