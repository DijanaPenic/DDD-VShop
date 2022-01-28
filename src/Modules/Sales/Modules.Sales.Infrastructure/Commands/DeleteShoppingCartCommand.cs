using System;

using VShop.SharedKernel.Infrastructure.Commands.Contracts;

namespace VShop.Modules.Sales.Infrastructure.Commands
{
    public partial class DeleteShoppingCartCommand : ICommand
    {
        public DeleteShoppingCartCommand(Guid shoppingCartId)
         {
             ShoppingCartId = shoppingCartId;
         }
    }
}