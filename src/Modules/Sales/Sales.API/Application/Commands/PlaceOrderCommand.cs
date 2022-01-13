using System;
using VShop.SharedKernel.Messaging.Commands;

namespace VShop.Modules.Sales.API.Application.Commands
{
    public partial class PlaceOrderCommand : IBaseCommand
    {
        public PlaceOrderCommand
        (
            Guid orderId,
            Guid shoppingCartId
        )
        {
            OrderId = orderId;
            ShoppingCartId = shoppingCartId;
        }
    }
}