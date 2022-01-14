using System;

using VShop.SharedKernel.Messaging;
using VShop.SharedKernel.Messaging.Events;

namespace VShop.Modules.Sales.Domain.Events
{
    public partial class ShoppingCartProductQuantityIncreasedDomainEvent : MessageContext, IDomainEvent
    {
        public ShoppingCartProductQuantityIncreasedDomainEvent
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