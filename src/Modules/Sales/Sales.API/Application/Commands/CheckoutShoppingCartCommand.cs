using System;

using VShop.SharedKernel.Messaging;
using VShop.SharedKernel.Messaging.Commands;

namespace VShop.Modules.Sales.API.Application.Commands
{
    public partial class CheckoutShoppingCartCommand : MessageContext, ICommand<CheckoutResponse>
    {
        public CheckoutShoppingCartCommand(Guid shoppingCartId, MessageMetadata metadata)
        {
            ShoppingCartId = shoppingCartId;
            Metadata = metadata;
        }
    }
}