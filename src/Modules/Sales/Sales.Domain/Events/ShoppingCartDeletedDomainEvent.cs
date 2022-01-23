using System;

using VShop.SharedKernel.Infrastructure.Events;
using VShop.SharedKernel.Infrastructure.Messaging;

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