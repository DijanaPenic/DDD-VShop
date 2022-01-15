using System;

using VShop.SharedKernel.Messaging;
using VShop.SharedKernel.Messaging.Events;
using VShop.SharedKernel.Infrastructure.Extensions;

namespace VShop.Modules.Sales.Domain.Events
{
    public partial class ShoppingCartDeliveryCostChangedDomainEvent : MessageContext, IDomainEvent
    {
        public ShoppingCartDeliveryCostChangedDomainEvent
        (
            Guid shoppingCartId,
            decimal deliveryCost
        )
        {
            ShoppingCartId = shoppingCartId;
            DeliveryCost = deliveryCost.ToMoney();
        }
    }
}