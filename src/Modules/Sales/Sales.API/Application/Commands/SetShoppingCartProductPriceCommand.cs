using System;

using VShop.SharedKernel.Messaging.Commands;

namespace VShop.Modules.Sales.API.Application.Commands
{
    public partial class SetShoppingCartProductPriceCommand : Command, IBaseCommand
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