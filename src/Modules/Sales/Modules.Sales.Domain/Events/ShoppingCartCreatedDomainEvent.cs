using System;

using VShop.SharedKernel.Infrastructure.Messaging;
using VShop.SharedKernel.Infrastructure.Events.Contracts;

namespace VShop.Modules.Sales.Domain.Events
{
    public partial class ShoppingCartCreatedDomainEvent : IDomainEvent
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