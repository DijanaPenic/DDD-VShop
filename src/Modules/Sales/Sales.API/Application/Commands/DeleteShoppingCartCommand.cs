using System;

namespace VShop.Modules.Sales.API.Application.Commands
{
    public partial class DeleteShoppingCartCommand
    {
        public DeleteShoppingCartCommand(Guid shoppingCartId)
         {
             ShoppingCartId = shoppingCartId;
         }
    }
}