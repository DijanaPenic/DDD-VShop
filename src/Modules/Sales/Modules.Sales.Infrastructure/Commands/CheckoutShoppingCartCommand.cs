using System;

using VShop.SharedKernel.Infrastructure.Commands.Contracts;
using VShop.Modules.Sales.Infrastructure.Commands.Handlers;

namespace VShop.Modules.Sales.Infrastructure.Commands
{
    internal partial class CheckoutShoppingCartCommand : ICommand<OrderInfo>
    {
        public CheckoutShoppingCartCommand(Guid shoppingCartId)
        {
            ShoppingCartId = shoppingCartId;
        }
    }
}