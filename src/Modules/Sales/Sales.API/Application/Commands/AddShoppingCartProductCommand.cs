using System;

using VShop.SharedKernel.Infrastructure.Commands;
using VShop.SharedKernel.Infrastructure.Messaging;
using VShop.Modules.Sales.API.Application.Commands.Shared;

namespace VShop.Modules.Sales.API.Application.Commands
{
    public partial class AddShoppingCartProductCommand : MessageContext, ICommand
    {
        public AddShoppingCartProductCommand
        (
            Guid shoppingCartId,
            ShoppingCartProductCommandDto shoppingCartItem,
            MessageMetadata metadata
        )
        {
            ShoppingCartId = shoppingCartId;
            ShoppingCartItem = shoppingCartItem;
            Metadata = metadata;
        }
    }
}