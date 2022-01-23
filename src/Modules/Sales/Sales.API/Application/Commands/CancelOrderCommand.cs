using System;

using VShop.SharedKernel.Infrastructure.Commands;
using VShop.SharedKernel.Infrastructure.Messaging;

namespace VShop.Modules.Sales.API.Application.Commands
{
    public partial class CancelOrderCommand : MessageContext, ICommand
    {
        public CancelOrderCommand(Guid orderId, MessageMetadata metadata)
        {
            OrderId = orderId;
            Metadata = metadata;
        }
    }
}