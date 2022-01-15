using System;

using VShop.SharedKernel.Messaging;
using VShop.SharedKernel.Messaging.Commands;

namespace VShop.Modules.Sales.API.Application.Commands
{
    public partial class SetPaidOrderStatusCommand : Command, IBaseCommand
    {
        public SetPaidOrderStatusCommand(Guid orderId, MessageMetadata metadata = default)
        {
            OrderId = orderId;
            Metadata = metadata;
        }
    }
}