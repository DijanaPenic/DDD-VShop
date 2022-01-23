using System;

using VShop.SharedKernel.Infrastructure.Events;
using VShop.SharedKernel.Infrastructure.Extensions;
using VShop.SharedKernel.Infrastructure.Messaging;

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