using System;

using VShop.SharedKernel.Infrastructure.Messaging;
using VShop.SharedKernel.Infrastructure.Commands.Contracts;

namespace VShop.Modules.Sales.Infrastructure.Commands
{
    public partial class DeleteShoppingCartCommand : MessageContext, ICommand
    {
        public DeleteShoppingCartCommand(Guid shoppingCartId, MessageMetadata metadata)
         {
             ShoppingCartId = shoppingCartId;
             Metadata = metadata;
         }
    }
}