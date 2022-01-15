using System;

using VShop.SharedKernel.Messaging;
using VShop.SharedKernel.Messaging.Commands;

namespace VShop.Modules.Sales.API.Application.Commands
{
    public partial class CheckoutShoppingCartCommand : Command<CheckoutResponse>, IBaseCommand
    {
        public CheckoutShoppingCartCommand(Guid shoppingCartId, MessageMetadata metadata = default)
        {
            ShoppingCartId = shoppingCartId;
            Metadata = metadata;
        }
    }
}