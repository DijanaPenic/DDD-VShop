using System;

using VShop.SharedKernel.Messaging.Commands;

namespace VShop.Modules.Sales.API.Application.Commands
{
    public partial class DeleteShoppingCartCommand : Command, IBaseCommand
    {
        public DeleteShoppingCartCommand(Guid shoppingCartId)
         {
             ShoppingCartId = shoppingCartId;
         }
    }
}