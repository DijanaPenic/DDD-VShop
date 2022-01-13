using System;

using VShop.SharedKernel.Messaging.Commands;

namespace VShop.Modules.Sales.API.Application.Commands
{
    public partial class CheckoutShoppingCartCommand : IBaseCommand
    {
        public CheckoutShoppingCartCommand(Guid shoppingCartId)
        {
            ShoppingCartId = shoppingCartId;
        }
    }
}