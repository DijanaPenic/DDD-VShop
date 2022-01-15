using System;

using VShop.SharedKernel.Messaging;
using VShop.SharedKernel.Messaging.Events;
using VShop.SharedKernel.Infrastructure.Extensions;

namespace VShop.Modules.Sales.Domain.Events
{
    public partial class ShoppingCartProductPriceChangedDomainEvent : MessageContext, IDomainEvent
    {
        public ShoppingCartProductPriceChangedDomainEvent
        (
            Guid shoppingCartId,
            Guid productId,
            decimal unitPrice
        )
        {
            ShoppingCartId = shoppingCartId;
            ProductId = productId;
            UnitPrice = unitPrice.ToMoney();
        }
    }
}