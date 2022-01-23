using System;

using VShop.SharedKernel.Infrastructure.Commands;
using VShop.SharedKernel.Infrastructure.Messaging;

namespace VShop.Modules.Sales.Core.Commands
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