using System;

using VShop.Modules.Sales.Infrastructure.Commands.Shared;
using VShop.SharedKernel.Infrastructure.Messaging;
using VShop.SharedKernel.Infrastructure.Commands.Contracts;

namespace VShop.Modules.Sales.Infrastructure.Commands
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