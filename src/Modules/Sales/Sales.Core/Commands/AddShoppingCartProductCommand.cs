using System;

using VShop.SharedKernel.Infrastructure.Commands;
using VShop.SharedKernel.Infrastructure.Messaging;
using VShop.Modules.Sales.Core.Commands.Shared;

namespace VShop.Modules.Sales.Core.Commands
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