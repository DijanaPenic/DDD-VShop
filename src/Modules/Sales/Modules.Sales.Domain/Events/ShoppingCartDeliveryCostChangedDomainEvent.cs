using System;

using VShop.SharedKernel.Infrastructure.Messaging;
using VShop.SharedKernel.Infrastructure.Extensions;
using VShop.SharedKernel.Infrastructure.Events.Contracts;

namespace VShop.Modules.Sales.Domain.Events
{
    internal partial class ShoppingCartDeliveryCostChangedDomainEvent : IDomainEvent
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