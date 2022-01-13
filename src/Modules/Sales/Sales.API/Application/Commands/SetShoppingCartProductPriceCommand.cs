using System;

namespace VShop.Modules.Sales.API.Application.Commands
{
    public partial class SetShoppingCartProductPriceCommand
    {
        public SetShoppingCartProductPriceCommand
        (
            Guid shoppingCartId,
            Guid productId,
            decimal unitPrice
        )
        {
            ShoppingCartId = shoppingCartId;
            ProductId = productId;
            UnitPrice = unitPrice;
        }
    }
}