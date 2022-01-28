using System;

using VShop.SharedKernel.Infrastructure.Commands.Contracts;

namespace VShop.Modules.Sales.Infrastructure.Commands
{
    public partial class RemoveShoppingCartProductCommand : ICommand
    {
        public RemoveShoppingCartProductCommand
        (
            Guid shoppingCartId,
            Guid productId,
            int quantity
        )
        {
            ShoppingCartId = shoppingCartId;
            ProductId = productId;
            Quantity = quantity;
        }
    }
}