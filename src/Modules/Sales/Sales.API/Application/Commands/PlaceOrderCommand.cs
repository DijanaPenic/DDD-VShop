using System;

using VShop.Modules.Sales.Domain.Models.Ordering;
using VShop.SharedKernel.Infrastructure.Commands;
using VShop.SharedKernel.Infrastructure.Messaging;

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