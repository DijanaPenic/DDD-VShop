using System;

using VShop.SharedKernel.Messaging.Commands;
using VShop.Modules.Sales.API.Application.Commands.Shared;

namespace VShop.Modules.Sales.API.Application.Commands
{
    public partial class AddShoppingCartProductCommand : IBaseCommand
    {
        public AddShoppingCartProductCommand
        (
            Guid shoppingCartId,
            ShoppingCartItemCommand shoppingCartItem
        )
        {
            ShoppingCartId = shoppingCartId;
            ShoppingCartItem = shoppingCartItem;
        }
    }
}