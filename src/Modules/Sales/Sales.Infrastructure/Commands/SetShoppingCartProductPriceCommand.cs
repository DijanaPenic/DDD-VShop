using System;

using VShop.SharedKernel.Infrastructure.Messaging;
using VShop.SharedKernel.Infrastructure.Extensions;
using VShop.SharedKernel.Infrastructure.Commands.Contracts;

namespace VShop.Modules.Sales.Infrastructure.Commands
{
    public partial class SetShoppingCartProductPriceCommand : MessageContext, ICommand
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