﻿using System;

using VShop.SharedKernel.Infrastructure.Messaging;
using VShop.SharedKernel.Infrastructure.Extensions;
using VShop.SharedKernel.Infrastructure.Events.Contracts;

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