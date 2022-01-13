using System;

namespace VShop.Modules.Sales.API.Application.Commands
{
    public partial class RemoveShoppingCartProductCommand
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