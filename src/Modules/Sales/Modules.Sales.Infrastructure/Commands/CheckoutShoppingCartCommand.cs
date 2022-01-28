using System;

using VShop.SharedKernel.Infrastructure.Commands.Contracts;
using VShop.Modules.Sales.Infrastructure.Commands.Handlers;

namespace VShop.Modules.Sales.Infrastructure.Commands
{
    public partial class CheckoutShoppingCartCommand : ICommand<CheckoutResponse>
    {
        public CheckoutShoppingCartCommand(Guid shoppingCartId)
        {
            ShoppingCartId = shoppingCartId;
        }
    }
}