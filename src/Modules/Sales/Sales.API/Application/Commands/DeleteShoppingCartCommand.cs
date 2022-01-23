using System;

using VShop.SharedKernel.Infrastructure.Commands;
using VShop.SharedKernel.Infrastructure.Messaging;

namespace VShop.Modules.Sales.API.Application.Commands
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