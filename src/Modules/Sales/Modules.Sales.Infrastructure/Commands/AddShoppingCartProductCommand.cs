using System;

using VShop.Modules.Sales.Infrastructure.Commands.Shared;
using VShop.SharedKernel.Infrastructure.Commands.Contracts;

namespace VShop.Modules.Sales.Infrastructure.Commands
{
    public partial class AddShoppingCartProductCommand : ICommand
    {
        public AddShoppingCartProductCommand
        (
            Guid shoppingCartId,
            ShoppingCartProductCommandDto shoppingCartItem
        )
        {
            ShoppingCartId = shoppingCartId;
            ShoppingCartItem = shoppingCartItem;
        }
    }
}