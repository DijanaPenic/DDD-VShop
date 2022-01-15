using System;

using VShop.SharedKernel.Messaging;
using VShop.SharedKernel.Messaging.Commands;
using VShop.SharedKernel.Infrastructure.Extensions;

namespace VShop.Modules.Sales.API.Application.Commands
{
    public partial class SetShoppingCartProductPriceCommand : Command, IBaseCommand
    {
        public SetShoppingCartProductPriceCommand
        (
            Guid shoppingCartId,
            Guid productId,
            decimal unitPrice,
            MessageMetadata metadata
        )
        {
            ShoppingCartId = shoppingCartId;
            ProductId = productId;
            UnitPrice = unitPrice.ToMoney();
            Metadata = metadata;
        }
    }
}