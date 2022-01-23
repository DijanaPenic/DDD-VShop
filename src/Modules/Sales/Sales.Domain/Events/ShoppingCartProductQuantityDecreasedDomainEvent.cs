using System;

using VShop.SharedKernel.Infrastructure.Events;
using VShop.SharedKernel.Infrastructure.Messaging;

namespace VShop.Modules.Sales.Domain.Events
{
    public partial class ShoppingCartProductQuantityDecreasedDomainEvent : MessageContext, IDomainEvent
    {
        public ShoppingCartProductQuantityDecreasedDomainEvent
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