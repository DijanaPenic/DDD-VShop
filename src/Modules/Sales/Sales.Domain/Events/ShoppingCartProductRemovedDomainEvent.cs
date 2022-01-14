using System;

using VShop.SharedKernel.Messaging;
using VShop.SharedKernel.Messaging.Events;

namespace VShop.Modules.Sales.Domain.Events
{
    public partial class ShoppingCartProductRemovedDomainEvent : MessageContext, IDomainEvent
    {
        public ShoppingCartProductRemovedDomainEvent
        (
            Guid shoppingCartId,
            Guid productId
        )
        {
            ShoppingCartId = shoppingCartId;
            ProductId = productId;
        }
    }
}