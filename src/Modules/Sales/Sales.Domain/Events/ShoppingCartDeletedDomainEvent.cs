using System;

using VShop.SharedKernel.Messaging;
using VShop.SharedKernel.Messaging.Events;

namespace VShop.Modules.Sales.Domain.Events
{
    public partial class ShoppingCartDeletedDomainEvent : MessageContext, IDomainEvent
    {
        public ShoppingCartDeletedDomainEvent(Guid shoppingCartId)
        {
            ShoppingCartId = shoppingCartId;
        }
    }
}