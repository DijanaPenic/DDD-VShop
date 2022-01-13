using System;

using VShop.SharedKernel.Messaging.Commands;

namespace VShop.Modules.Sales.API.Application.Commands
{
    public partial class CancelOrderCommand : IBaseCommand
    {
        public CancelOrderCommand(Guid orderId)
        {
            OrderId = orderId;
        }
    }
}