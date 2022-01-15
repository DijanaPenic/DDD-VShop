using System;

using VShop.SharedKernel.Messaging;
using VShop.SharedKernel.Messaging.Commands;
using VShop.Modules.Sales.API.Application.Commands.Shared;

namespace VShop.Modules.Sales.API.Application.Commands
{
    public partial class AddShoppingCartProductCommand : Command, IBaseCommand
    {
        public AddShoppingCartProductCommand
        (
            Guid shoppingCartId,
            ShoppingCartItemCommand shoppingCartItem,
            MessageMetadata metadata = default
        )
        {
            ShoppingCartId = shoppingCartId;
            ShoppingCartItem = shoppingCartItem;
            Metadata = metadata;
        }
    }
}