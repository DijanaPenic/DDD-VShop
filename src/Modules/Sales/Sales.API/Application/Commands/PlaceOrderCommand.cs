using System;

using VShop.SharedKernel.Messaging;
using VShop.SharedKernel.Messaging.Commands;
using VShop.Modules.Sales.Domain.Models.Ordering;

namespace VShop.Modules.Sales.API.Application.Commands
{
    public partial class PlaceOrderCommand : Command<Order>, IBaseCommand
    {
        public PlaceOrderCommand
        (
            Guid orderId,
            Guid shoppingCartId,
            MessageMetadata metadata = default
        )
        {
            OrderId = orderId;
            ShoppingCartId = shoppingCartId;
            Metadata = metadata;
        }
    }
}