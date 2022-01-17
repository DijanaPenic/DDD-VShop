using System;

using VShop.SharedKernel.Messaging;
using VShop.SharedKernel.Messaging.Commands;

namespace VShop.Modules.Sales.API.Application.Commands
{
    public partial class RemoveShoppingCartProductCommand : MessageContext, ICommand
    {
        public RemoveShoppingCartProductCommand
        (
            Guid shoppingCartId,
            Guid productId,
            int quantity,
            MessageMetadata metadata
        )
        {
            ShoppingCartId = shoppingCartId;
            ProductId = productId;
            Quantity = quantity;
            Metadata = metadata;
        }
    }
}