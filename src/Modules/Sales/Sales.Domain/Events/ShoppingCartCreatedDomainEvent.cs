using System;

using VShop.SharedKernel.Messaging;
using VShop.SharedKernel.Messaging.Events;

namespace VShop.Modules.Sales.Domain.Events
{
    public partial class ShoppingCartCreatedDomainEvent : MessageContext, IDomainEvent
    {
        public ShoppingCartCreatedDomainEvent
        (
            Guid shoppingCartId,
            Guid customerId,
            int customerDiscount
        )
        {
            ShoppingCartId = shoppingCartId;
            CustomerId = customerId;
            CustomerDiscount = customerDiscount;
        }
    }
}