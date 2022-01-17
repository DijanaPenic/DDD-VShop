using System;

using VShop.SharedKernel.Messaging;
using VShop.SharedKernel.Messaging.Commands;
using VShop.Modules.Sales.Domain.Models.Ordering;

namespace VShop.Modules.Sales.API.Application.Commands
{
    public partial class PlaceOrderCommand : MessageContext, ICommand<Order>
    {
        public PlaceOrderCommand
        (
            Guid orderId,
            Guid shoppingCartId,
            MessageMetadata metadata
        )
        {
            OrderId = orderId;
            ShoppingCartId = shoppingCartId;
            Metadata = metadata;
        }
    }
}