using System;

using VShop.SharedKernel.Infrastructure.Messaging;
using VShop.SharedKernel.Infrastructure.Events.Contracts;

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