using System;

using VShop.SharedKernel.Infrastructure.Events;
using VShop.SharedKernel.Infrastructure.Messaging;

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