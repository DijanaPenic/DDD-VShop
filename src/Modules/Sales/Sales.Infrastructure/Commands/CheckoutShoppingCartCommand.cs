using System;

using VShop.SharedKernel.Infrastructure.Messaging;
using VShop.SharedKernel.Infrastructure.Commands.Contracts;
using VShop.Modules.Sales.Infrastructure.Commands.Handlers;

namespace VShop.Modules.Sales.Infrastructure.Commands
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