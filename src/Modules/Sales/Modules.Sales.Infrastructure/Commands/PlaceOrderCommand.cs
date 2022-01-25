using System;

using VShop.Modules.Sales.Domain.Models.Ordering;
using VShop.SharedKernel.Infrastructure.Messaging;
using VShop.SharedKernel.Infrastructure.Commands.Contracts;

namespace VShop.Modules.Sales.Infrastructure.Commands
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